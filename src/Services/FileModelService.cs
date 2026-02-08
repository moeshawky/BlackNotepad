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
            // Use StreamReader.ReadToEnd() which is significantly faster than character-by-character reading
            using (var sr = new StreamReader(fileModel.Location))
            {
                content = sr.ReadToEnd();
                sr.Close();
            }

            // Legacy behavior: Empty files return \uFFFF to mimic original loop side-effect
            if (content.Length == 0)
            {
                content = "\uffff";
            }

            var lineEnding = LineEndings._;

            // Optimized line ending detection matching legacy precedence:
            // 1. Prioritize CRLF if present anywhere
            // 2. Fallback to checking end of file for LF or CR
            if (content.Contains("\r\n"))
            {
                lineEnding = LineEndings.CRLF;
            }
            else if (content.EndsWith("\n", System.StringComparison.Ordinal))
            {
                lineEnding = LineEndings.LF;
            }
            else if (content.EndsWith("\r", System.StringComparison.Ordinal))
            {
                lineEnding = LineEndings.CR;
            }

            fileModel.LineEnding = lineEnding;
            fileModel.Content = content;
            fileModel.IsDirty = false;
        }
    }
}
