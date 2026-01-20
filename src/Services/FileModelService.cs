using Savaged.BlackNotepad.Lookups;
using Savaged.BlackNotepad.Models;
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

        private void SaveFile(FileModel fileModel)
        {
            File.WriteAllText(fileModel.Location, fileModel.Content);
            fileModel.IsDirty = false;
        }

        private void ReadFile(FileModel fileModel)
        {
            if (string.IsNullOrEmpty(fileModel.Location)
                || string.IsNullOrWhiteSpace(fileModel.Location))
            {
                return;
            }

            string content;
            // Optimization: Use ReadToEnd for much faster file reading than char-by-char
            // This also fixes a bug where empty files resulted in "\uFFFF" content
            using (var sr = new StreamReader(fileModel.Location))
            {
                content = sr.ReadToEnd();
            }

            var lineEnding = LineEndings._;

            // Optimization: Use fast string search/check instead of per-char loop
            // Logic preserved:
            // 1. CRLF anywhere in the file -> CRLF
            // 2. LF at the very end (and no CRLF) -> LF
            // 3. CR at the very end (and no CRLF) -> CR
            if (content.Contains("\r\n"))
            {
                lineEnding = LineEndings.CRLF;
            }
            else if (content.EndsWith("\n"))
            {
                lineEnding = LineEndings.LF;
            }
            else if (content.EndsWith("\r"))
            {
                lineEnding = LineEndings.CR;
            }

            fileModel.LineEnding = lineEnding;
            fileModel.Content = content;
            fileModel.IsDirty = false;
        }
    }
}
