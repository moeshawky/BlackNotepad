using Savaged.BlackNotepad.Lookups;
using Savaged.BlackNotepad.Models;
using System.IO;
using System.Text;
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

            // Optimization: Use ReadToEnd for faster I/O instead of char-by-char reading
            string content;
            using (var sr = new StreamReader(fileModel.Location))
            {
                content = sr.ReadToEnd();
                sr.Close(); // Explicit Close to match legacy style
            }

            // Legacy quirk: Empty file returns \uFFFF
            if (content.Length == 0)
            {
                content = unchecked((char)-1).ToString();
            }

            // Legacy detection logic preserved:
            // CRLF detected anywhere in file
            // LF or CR only detected if at the very end (and no CRLF found)
            var lineEnding = LineEndings._;
            if (content.Contains("\r\n"))
            {
                lineEnding = LineEndings.CRLF;
            }
            else if (content.EndsWith("\n", System.StringComparison.Ordinal))
            {
                lineEnding = LineEndings.LF;
            }
            else if (content.EndsWith("\r", System.StringComparison.Ordinal))
            {
                lineEnding = LineEndings.CR;
            }

            fileModel.LineEnding = lineEnding;
            fileModel.Content = content;
            fileModel.IsDirty = false;
        }
    }
}
