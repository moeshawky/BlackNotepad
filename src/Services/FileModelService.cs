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

            var lineEnding = LineEndings._;
            using (var sr = new StreamReader(fileModel.Location))
            {
                fileModel.Content = sr.ReadToEnd();
            }

            // Optimization: Detect line endings from the loaded string
            // instead of character-by-character reading which is slow.
            if (fileModel.Content.Contains("\r\n"))
            {
                lineEnding = LineEndings.CRLF;
            }
            else if (fileModel.Content.EndsWith("\n"))
            {
                lineEnding = LineEndings.LF;
            }
            else if (fileModel.Content.EndsWith("\r"))
            {
                lineEnding = LineEndings.CR;
            }

            fileModel.LineEnding = lineEnding;
            fileModel.IsDirty = false;
        }
    }
}
