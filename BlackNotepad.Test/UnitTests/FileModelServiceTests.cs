using Microsoft.VisualStudio.TestTools.UnitTesting;
using Savaged.BlackNotepad.Services;
using Savaged.BlackNotepad.Lookups;
using Savaged.BlackNotepad.Models;
using System.IO;
using System.Threading.Tasks;

namespace BlackNotepad.Test.UnitTests
{
    [TestClass]
    public class FileModelServiceTests
    {
        private string _tempFile;

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_tempFile))
            {
                File.Delete(_tempFile);
            }
        }

        [TestMethod]
        public async Task LoadAsync_ReadsCRLF()
        {
            _tempFile = Path.GetTempFileName();
            File.WriteAllText(_tempFile, "Line1\r\nLine2");

            var service = new FileModelService();
            var model = await service.LoadAsync(_tempFile);

            Assert.AreEqual("Line1\r\nLine2", model.Content);
            Assert.AreEqual(LineEndings.CRLF, model.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_ReadsLF()
        {
            _tempFile = Path.GetTempFileName();
            File.WriteAllText(_tempFile, "Line1\nLine2");

            var service = new FileModelService();
            var model = await service.LoadAsync(_tempFile);

            Assert.AreEqual("Line1\nLine2", model.Content);
            Assert.AreEqual(LineEndings.LF, model.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_ReadsCR()
        {
            _tempFile = Path.GetTempFileName();
            File.WriteAllText(_tempFile, "Line1\rLine2");

            var service = new FileModelService();
            var model = await service.LoadAsync(_tempFile);

            Assert.AreEqual("Line1\rLine2", model.Content);
            Assert.AreEqual(LineEndings.CR, model.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_Mixed_CRLF_TakesPrecedence()
        {
            _tempFile = Path.GetTempFileName();
            File.WriteAllText(_tempFile, "Line1\r\nLine2\nLine3\r");

            var service = new FileModelService();
            var model = await service.LoadAsync(_tempFile);

            Assert.AreEqual(LineEndings.CRLF, model.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_Mixed_LF_TakesPrecedenceOverCR()
        {
            _tempFile = Path.GetTempFileName();
            File.WriteAllText(_tempFile, "Line1\nLine2\r");

            var service = new FileModelService();
            var model = await service.LoadAsync(_tempFile);

            Assert.AreEqual(LineEndings.LF, model.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_EmptyFile()
        {
            _tempFile = Path.GetTempFileName();
            // Create empty file
            File.WriteAllText(_tempFile, "");

            var service = new FileModelService();
            var model = await service.LoadAsync(_tempFile);

            Assert.AreEqual("", model.Content);
            Assert.AreEqual(LineEndings._, model.LineEnding);
        }
    }
}
