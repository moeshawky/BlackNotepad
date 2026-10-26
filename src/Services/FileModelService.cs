using GalaSoft.MvvmLight.Ioc;
using Savaged.BlackNotepad.Lookups;
using Savaged.BlackNotepad.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Savaged.BlackNotepad.Services
{
    /// <summary>
    /// Service responsible for file creation, asynchronous loading, atomic disk persistence,
    /// and local document history coordination.
    /// </summary>
    public class FileModelService : IFileModelService
    {
        private readonly IFileHistoryService _historyService;
        private readonly SemaphoreSlim _saveLock = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Initializes a new instance of the <see cref="FileModelService"/> class with default dependencies.
        /// </summary>
        public FileModelService()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileModelService"/> class with the specified history service.
        /// </summary>
        /// <param name="historyService">The history service to record document revisions, or null to disable history recording.</param>
        [PreferredConstructor]
        public FileModelService(IFileHistoryService historyService)
        {
            _historyService = historyService;
        }

        /// <summary>
        /// Creates a new empty FileModel with default values (Name="Untitled", Content="", IsDirty=false).
        /// </summary>
        /// <returns>A new FileModel instance. Caller owns the returned reference.</returns>
        public FileModel New()
        {
            return new FileModel();
        }

        /// <summary>
        /// Loads file content from disk and detects line endings on a background thread,
        /// then assigns properties on the UI thread after await completes.
        /// </summary>
        /// <param name="location">File path to read. If null, empty, or whitespace, returns a FileModel with empty Content.</param>
        /// <returns>
        /// A FileModel with Location, Name, Content, LineEnding, and IsDirty=false populated.
        /// Returns a FileModel with empty Content if location is null/whitespace or file is empty.
        /// </returns>
        /// <exception cref="System.IO.FileNotFoundException">Thrown if the file does not exist.</exception>
        /// <exception cref="System.IO.IOException">Thrown if file cannot be read.</exception>
        public async Task<FileModel> LoadAsync(string location)
        {
            var fileModel = new FileModel
            {
                Location = location
            };

            string content = null;
            LineEndings lineEnding = LineEndings._;

            await Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(fileModel.Location))
                {
                    return;
                }

                using (var sr = new StreamReader(fileModel.Location, true))
                {
                    content = sr.ReadToEnd();
                }

                int rIndex = content.IndexOf('\r');
                int nIndex = content.IndexOf('\n');

                if (rIndex != -1 && nIndex != -1)
                {
                    if (rIndex < nIndex)
                    {
                        if (rIndex + 1 == nIndex)
                        {
                            lineEnding = LineEndings.CRLF;
                        }
                        else
                        {
                            lineEnding = LineEndings.CR;
                        }
                    }
                    else
                    {
                        lineEnding = LineEndings.LF;
                    }
                }
                else if (rIndex != -1)
                {
                    lineEnding = LineEndings.CR;
                }
                else if (nIndex != -1)
                {
                    lineEnding = LineEndings.LF;
                }
            });

            if (content != null)
            {
                fileModel.LineEnding = lineEnding;
                fileModel.Content = content;
                fileModel.IsDirty = false;
            }

            return fileModel;
        }

        /// <summary>
        /// Saves file content to disk atomically on a background thread using a default manual save reason.
        /// </summary>
        /// <param name="fileModel">The file model to save. Must have non-null Location and Content.</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        public Task SaveAsync(FileModel fileModel)
        {
            return SaveAsync(fileModel, SaveReason.ManualSave);
        }

        /// <summary>
        /// Saves file content to disk atomically (write-to-temp-then-replace) on a background thread.
        /// Serializes concurrent saves via SemaphoreSlim, captures an immutable FileSaveSnapshot to eliminate
        /// TOCTOU races, records document history, and clears IsDirty only if Content is unchanged.
        /// </summary>
        /// <param name="fileModel">The file model to save. Must have non-null Location and Content.</param>
        /// <param name="reason">The reason triggering the save operation.</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if fileModel is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if fileModel.Location is null or empty.</exception>
        /// <exception cref="System.IO.IOException">Thrown if file cannot be written to disk.</exception>
        public async Task SaveAsync(FileModel fileModel, SaveReason reason)
        {
            if (fileModel == null)
            {
                throw new ArgumentNullException(nameof(fileModel));
            }
            if (string.IsNullOrWhiteSpace(fileModel.Location))
            {
                throw new ArgumentException("File location cannot be null or empty.", nameof(fileModel));
            }

            // Capture immutable snapshot BEFORE acquiring locks or running background tasks
            // to eliminate TOCTOU mutation races caused by concurrent user typing.
            string capturedContent = fileModel.Content ?? string.Empty;
            var snapshot = new FileSaveSnapshot(fileModel.Location, capturedContent, DateTime.UtcNow, reason);

            await _saveLock.WaitAsync();
            try
            {
                await Task.Run(async () =>
                {
                    // Detect external changes on disk before overwriting, and preserve them in history
                    if (File.Exists(snapshot.Location) && _historyService != null)
                    {
                        try
                        {
                            string diskContent = File.ReadAllText(snapshot.Location);
                            await _historyService.RecordExternalBaselineIfNeededAsync(snapshot.Location, diskContent).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Failed to record external baseline: {ex.Message}");
                        }
                    }

                    // Write snapshot content to disk atomically
                    SaveContentToFile(snapshot.Location, snapshot.Content);

                    // Record snapshot in local history
                    if (_historyService != null)
                    {
                        try
                        {
                            await _historyService.SaveSnapshotAsync(snapshot).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Failed to record history snapshot: {ex.Message}");
                        }
                    }
                });
            }
            finally
            {
                _saveLock.Release();
            }

            // Clear dirty state only if file content has not been mutated while save was in flight
            if (fileModel.Content == snapshot.Content)
            {
                fileModel.IsDirty = false;
            }
        }

        /// <summary>
        /// Writes file content atomically using write-to-temp-then-replace pattern.
        /// Writes to a .tmp file first, then replaces the original. This prevents
        /// data loss on power failure or crash during write — the original file
        /// remains intact until the temp file is fully written.
        /// </summary>
        /// <param name="location">The destination file path.</param>
        /// <param name="content">The text content to write.</param>
        private void SaveContentToFile(string location, string content)
        {
            var tmpPath = location + ".tmp";
            var backupPath = location + ".bak";

            try
            {
                File.WriteAllText(tmpPath, content);

                if (File.Exists(location))
                {
                    File.Replace(tmpPath, location, backupPath);
                    File.Delete(backupPath);
                }
                else
                {
                    File.Move(tmpPath, location);
                }
            }
            catch (Exception)
            {
                if (File.Exists(tmpPath))
                {
                    try { File.Delete(tmpPath); }
                    catch (Exception deleteEx)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            $"Failed to delete temp file '{tmpPath}': {deleteEx.Message}");
                    }
                }
                throw;
            }
        }
    }
}
