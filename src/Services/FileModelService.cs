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
            using (var sr = new StreamReader(fileModel.Location))
            {
                content = sr.ReadToEnd();
            }

            fileModel.Content = content;
            fileModel.IsDirty = false;

            var lineEnding = LineEndings._;

            int firstCr = content.IndexOf('\r');
            int firstLf = content.IndexOf('\n');

            if (firstCr == -1 && firstLf == -1)
            {
                lineEnding = LineEndings._;
            }
            else if (firstCr != -1 && (firstLf == -1 || firstCr < firstLf))
            {
                if (firstCr + 1 < content.Length && content[firstCr + 1] == '\n')
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

            fileModel.LineEnding = lineEnding;
        }
    }
}
