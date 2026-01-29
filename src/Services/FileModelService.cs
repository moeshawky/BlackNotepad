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
            using (var sr = new StreamReader(fileModel.Location))
            {
                content = sr.ReadToEnd();
                sr.Close();
            }

            // Optimization note: ReadToEnd() is significantly faster than reading char-by-char.
            // Legacy compatibility: Empty files previously produced "\uFFFF" due to StreamReader.Read() returning -1.
            if (content.Length == 0)
            {
                content = unchecked((char)-1).ToString();
            }

            fileModel.Content = content;

            // Legacy line ending detection logic:
            // - CRLF is detected anywhere.
            // - LF/CR are only detected if they appear at the very end of the file.
            if (content.Contains("\r\n"))
            {
                fileModel.LineEnding = LineEndings.CRLF;
            }
            else if (content.EndsWith("\n"))
            {
                fileModel.LineEnding = LineEndings.LF;
            }
            else if (content.EndsWith("\r"))
            {
                fileModel.LineEnding = LineEndings.CR;
            }
            else
            {
                fileModel.LineEnding = LineEndings._;
            }

            fileModel.IsDirty = false;
        }
    }
}
