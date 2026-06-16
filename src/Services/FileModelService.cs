using Savaged.BlackNotepad.Lookups;
using Savaged.BlackNotepad.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Savaged.BlackNotepad.Services
{
    public class FileModelService : IFileModelService
    {
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
        /// Saves file content to disk atomically (write-to-temp-then-replace) on a background thread,
        /// then sets IsDirty=false on the UI thread after await completes.
        /// </summary>
        /// <param name="fileModel">The file model to save. Must have non-null Location and Content.</param>
        /// <exception cref="System.ArgumentException">Thrown if fileModel.Location is null or empty.</exception>
        /// <exception cref="System.IO.IOException">Thrown if file cannot be written to disk.</exception>
        public async Task SaveAsync(FileModel fileModel)
        {
            await Task.Run(() => SaveFile(fileModel));
            fileModel.IsDirty = false;
        }

        /// <summary>
        /// Writes file content atomically using write-to-temp-then-replace pattern.
        /// Writes to a .tmp file first, then replaces the original. This prevents
        /// data loss on power failure or crash during write — the original file
        /// remains intact until the temp file is fully written.
        /// </summary>
        /// <param name="fileModel">The file model containing Location and Content to save.</param>
        private void SaveFile(FileModel fileModel)
        {
            var tmpPath = fileModel.Location + ".tmp";
            var backupPath = fileModel.Location + ".bak";

            try
            {
                File.WriteAllText(tmpPath, fileModel.Content);

                if (File.Exists(fileModel.Location))
                {
                    File.Replace(tmpPath, fileModel.Location, backupPath);
                    File.Delete(backupPath);
                }
                else
                {
                    File.Move(tmpPath, fileModel.Location);
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
