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
            var lineEnding = LineEndings._;

            // PERFORMANCE OPTIMIZATION:
            // Replaced character-by-character reading loop with StreamReader.ReadToEnd().
            // This significantly reduces I/O overhead and string allocation costs for large files.
            using (var sr = new StreamReader(fileModel.Location))
            {
                content = sr.ReadToEnd();
            }

            // Detect line endings based on precedence: CRLF > LF > CR
            if (content.Contains("\r\n"))
            {
                lineEnding = LineEndings.CRLF;
            }
            else if (content.Contains("\n"))
            {
                lineEnding = LineEndings.LF;
            }
            else if (content.Contains("\r"))
            {
                lineEnding = LineEndings.CR;
            }

            fileModel.LineEnding = lineEnding;
            fileModel.Content = content;
            fileModel.IsDirty = false;
        }
    }
}
