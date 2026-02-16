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
            // Use StreamReader to preserve encoding detection
            using (var sr = new StreamReader(fileModel.Location))
            {
                content = sr.ReadToEnd();
                sr.Close();
            }

            // Detect line endings using the first occurrence found
            var lineEnding = LineEndings._;
            int idx = content.IndexOfAny(new char[] { '\r', '\n' });
            if (idx != -1)
            {
                if (content[idx] == '\r')
                {
                    if (idx + 1 < content.Length && content[idx + 1] == '\n')
                    {
                        lineEnding = LineEndings.CRLF;
                    }
                    else
                    {
                        lineEnding = LineEndings.CR;
                    }
                }
                else // content[idx] == '\n'
                {
                    lineEnding = LineEndings.LF;
                }
            }

            fileModel.LineEnding = lineEnding;
            fileModel.Content = content;
            fileModel.IsDirty = false;
        }
    }
}
