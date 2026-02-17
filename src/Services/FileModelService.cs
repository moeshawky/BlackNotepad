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

            // Bolt: Optimized line ending detection using IndexOfAny
            var index = content.IndexOfAny(new[] { '\r', '\n' });
            if (index == -1)
            {
                fileModel.LineEnding = LineEndings._;
            }
            else
            {
                if (content[index] == '\r')
                {
                    if (index + 1 < content.Length && content[index + 1] == '\n')
                    {
                        fileModel.LineEnding = LineEndings.CRLF;
                    }
                    else
                    {
                        fileModel.LineEnding = LineEndings.CR;
                    }
                }
                else
                {
                    fileModel.LineEnding = LineEndings.LF;
                }
            }
        }
    }
}
