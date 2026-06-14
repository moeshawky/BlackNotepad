using Savaged.BlackNotepad.Lookups;
using Savaged.BlackNotepad.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Savaged.BlackNotepad.Services
{
    public class FileModelService : IFileModelService
    {
        public FileModel New()
        {
            return new FileModel();
        }

        public async Task<FileModel> LoadAsync(string location)
        {
            var fileModel = new FileModel
            {
                Location = location
            };
            await Task.Run(() => ReadFile(fileModel));
            
            return fileModel;
        }

        public async Task SaveAsync(FileModel fileModel)
        {
            await Task.Run(() => SaveFile(fileModel));
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

                fileModel.IsDirty = false;
            }
            catch (Exception)
            {
                if (File.Exists(tmpPath))
                {
                    try { File.Delete(tmpPath); } catch { }
                }
                throw;
            }
        }

        private void ReadFile(FileModel fileModel)
        {
            if (string.IsNullOrEmpty(fileModel.Location)
                || string.IsNullOrWhiteSpace(fileModel.Location))
            {
                return;
            }

            string content;
            using (var sr = new StreamReader(fileModel.Location, true))
            {
                content = sr.ReadToEnd();
            }

            var lineEnding = LineEndings._;

            // Bolt: Optimized from char-by-char loop to IndexOf scan.
            // Also fixes bug where LF files were not detected correctly unless ending with \n.
            int rIndex = content.IndexOf('\r');
            int nIndex = content.IndexOf('\n');

            if (rIndex != -1 && nIndex != -1)
            {
                if (rIndex < nIndex)
                {
                    // \r appears before \n
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
                    // \n appears before \r
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

            fileModel.LineEnding = lineEnding;
            fileModel.Content = content;
            fileModel.IsDirty = false;
        }
    }
}
