using Microsoft.VisualStudio.TestTools.UnitTesting;
using Savaged.BlackNotepad.Lookups;
using Savaged.BlackNotepad.Models;
using Savaged.BlackNotepad.Services;
using System.IO;
using System.Threading.Tasks;

namespace BlackNotepad.Test.UnitTests
{
    [TestClass]
    public class FileModelServiceTests
    {
        private const string TestContent = "Hello World";
        private string _tempFile;

        [TestInitialize]
        public void Init()
        {
            _tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_tempFile))
            {
                File.Delete(_tempFile);
            }
        }

        [TestMethod]
        public async Task LoadAsync_ShouldReadFileContent()
        {
            File.WriteAllText(_tempFile, TestContent);

            var service = new FileModelService();
            var model = await service.LoadAsync(_tempFile);

            Assert.AreEqual(TestContent, model.Content);
            Assert.IsFalse(model.IsDirty);
        }

        [TestMethod]
        public async Task LoadAsync_ShouldDetectCRLF()
        {
            File.WriteAllText(_tempFile, "Line1\r\nLine2");

            var service = new FileModelService();
            var model = await service.LoadAsync(_tempFile);

            Assert.AreEqual(LineEndings.CRLF, model.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_ShouldDetectLF_AtEnd()
        {
            // Note: Legacy logic only detects LF if it's the last char
            File.WriteAllText(_tempFile, "Line1\n");

            var service = new FileModelService();
            var model = await service.LoadAsync(_tempFile);

            Assert.AreEqual(LineEndings.LF, model.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_ShouldDetectCR_AtEnd()
        {
            File.WriteAllText(_tempFile, "Line1\r");

            var service = new FileModelService();
            var model = await service.LoadAsync(_tempFile);

            Assert.AreEqual(LineEndings.CR, model.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_ShouldHandleEmptyFile()
        {
            File.WriteAllText(_tempFile, "");

            var service = new FileModelService();
            var model = await service.LoadAsync(_tempFile);

            // Legacy behavior: returns \uffff
            Assert.AreEqual("\uffff", model.Content);
            Assert.IsFalse(model.IsDirty);
        }
    }
}
