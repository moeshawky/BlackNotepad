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

            // Optimization: Use ReadToEnd for bulk reading instead of character-by-character
            // to improve performance and avoid overhead.
            string content;
            using (var sr = new StreamReader(fileModel.Location))
            {
                content = sr.ReadToEnd();
                sr.Close(); // Explicit close matching legacy style
            }

            // Legacy Compatibility: The previous implementation using StreamReader.Read() in a loop
            // would append (char)-1 (\uFFFF) if the file was empty (immediate EOF).
            // We replicate this behavior to ensure strict backward compatibility.
            if (string.IsNullOrEmpty(content))
            {
                content = ((char)-1).ToString();
            }

            // Preserve legacy line ending detection logic:
            // - CRLF is detected anywhere
            // - LF or CR are only detected if they appear at the very end
            var lineEnding = LineEndings._;
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

            fileModel.LineEnding = lineEnding;
            fileModel.Content = content;
            fileModel.IsDirty = false;
        }
    }
}
