using Savaged.BlackNotepad.Lookups;
using Savaged.BlackNotepad.Models;
using System;
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
            using (var sr = new StreamReader(fileModel.Location))
            {
                // Optimization: ReadToEnd is significantly faster than reading char-by-char.
                content = sr.ReadToEnd();
                sr.Close();
            }

            // Legacy behavior: Empty files are represented by a single \uFFFF char.
            // This is preserved to match the behavior of the previous char-by-char reading loop
            // where sr.Read() returned -1 (0xFFFF) which was cast to char and appended.
            if (content.Length == 0)
            {
                content = unchecked((char)-1).ToString();
            }

            var lineEnding = LineEndings._;

            // Optimization: Check for line endings on the full string.
            // Logic matches previous implementation:
            // 1. CRLF is detected anywhere in the file.
            // 2. LF or CR are ONLY detected if they are the very last character (EOF).
            if (content.Contains("\r\n"))
            {
                lineEnding = LineEndings.CRLF;
            }
            else if (content.EndsWith("\n", StringComparison.Ordinal))
            {
                lineEnding = LineEndings.LF;
            }
            else if (content.EndsWith("\r", StringComparison.Ordinal))
            {
                lineEnding = LineEndings.CR;
            }

            fileModel.LineEnding = lineEnding;
            fileModel.Content = content;
            fileModel.IsDirty = false;
        }
    }
}
