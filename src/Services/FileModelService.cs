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

            var lineEnding = LineEndings._;

            // Bolt: Optimized from char-by-char loop to IndexOf scan.
            // Also fixes bug where LF files were not detected correctly unless ending with \n.
            int rIndex = content.IndexOf('\r');
            int nIndex = content.IndexOf('\n');

            if (rIndex != -1 && nIndex != -1)
            {
                if (rIndex < nIndex)
                {
                    // \r appears before \n
                    if (rIndex + 1 == nIndex)
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
                    // \n appears before \r
                    lineEnding = LineEndings.LF;
                }
            }
            else if (rIndex != -1)
            {
                lineEnding = LineEndings.CR;
            }
            else if (nIndex != -1)
            {
                lineEnding = LineEndings.LF;
            }

            fileModel.LineEnding = lineEnding;
            fileModel.Content = content;
            fileModel.IsDirty = false;
        }
    }
}
