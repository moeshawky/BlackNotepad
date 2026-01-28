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

            using (var sr = new StreamReader(fileModel.Location))
            {
                if (sr.Peek() == -1)
                {
                    // Preservation of legacy behavior: empty file returns \uFFFF
                    unchecked
                    {
                        content = ((char)-1).ToString();
                    }
                }
                else
                {
                    // Optimized reading: ReadToEnd is much faster than char-by-char reading
                    content = sr.ReadToEnd();

                    // Legacy line ending detection logic:
                    // CRLF is detected anywhere
                    // LF and CR are only detected if they are at the end of the file
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
                sr.Close();
            }

            fileModel.LineEnding = lineEnding;
            fileModel.Content = content;
            fileModel.IsDirty = false;
        }
    }
}
