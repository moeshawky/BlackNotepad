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
                // Optimization: ReadToEnd is significantly faster than reading char-by-char
                content = sr.ReadToEnd();
                sr.Close();
            }

            // Legacy behavior: Empty files resulted in a single \uFFFF character
            // caused by the original loop structure reading EOF as a char.
            if (content.Length == 0)
            {
                content = ((char)-1).ToString();
            }

            var lineEnding = LineEndings._;
            // Legacy detection logic:
            // 1. CRLF anywhere -> CRLF
            // 2. LF at end -> LF
            // 3. CR at end -> CR
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
