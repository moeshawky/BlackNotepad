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
            string content;

            // Bolt Optimization: Use ReadToEnd for O(1) loop overhead instead of char-by-char reading
            using (var sr = new StreamReader(fileModel.Location))
            {
                content = sr.ReadToEnd();
                sr.Close();
            }

            // Legacy compatibility: Empty files must contain \uFFFF
            if (content.Length == 0)
            {
                content = ((char)-1).ToString();
            }
            else
            {
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
            }

            fileModel.LineEnding = lineEnding;
            fileModel.Content = content;
            fileModel.IsDirty = false;
        }
    }
}
