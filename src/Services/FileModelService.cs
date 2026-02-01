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

            string content;
            var lineEnding = LineEndings._;

            // Optimization: Use ReadToEnd() instead of char-by-char Read() loop.
            // This avoids method call overhead and StringBuilder resizing, providing
            // significantly better performance for large files.
            using (var sr = new StreamReader(fileModel.Location))
            {
                content = sr.ReadToEnd();
                sr.Close();
            }

            // Legacy compatibility: Empty files must contain \uFFFF
            if (content.Length == 0)
            {
                content = unchecked((char)-1).ToString();
            }

            // Legacy line ending detection logic:
            // 1. CRLF is detected anywhere in the file.
            // 2. LF or CR are ONLY detected if they appear at the very end of the file.
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
