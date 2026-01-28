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
        private FileModelService _service;
        private string _tempFile;

        [TestInitialize]
        public void Setup()
        {
            _service = new FileModelService();
            _tempFile = Path.Combine(Path.GetTempPath(), "FileModelServiceTest_" + System.Guid.NewGuid().ToString() + ".txt");
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
        public async Task LoadAsync_EmptyFile_ReturnsFFFFAndUnderscoreLineEnding()
        {
            File.WriteAllText(_tempFile, "");
            var fileModel = await _service.LoadAsync(_tempFile);

            unchecked
            {
                char expectedChar = (char)-1;
                Assert.AreEqual(expectedChar.ToString(), fileModel.Content);
            }
            Assert.AreEqual(LineEndings._, fileModel.LineEnding);
            Assert.IsFalse(fileModel.IsDirty);
        }

        [TestMethod]
        public async Task LoadAsync_CRLF_DetectsCRLF()
        {
            File.WriteAllText(_tempFile, "Hello\r\nWorld");
            var fileModel = await _service.LoadAsync(_tempFile);

            Assert.AreEqual("Hello\r\nWorld", fileModel.Content);
            Assert.AreEqual(LineEndings.CRLF, fileModel.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_EndsWithLF_DetectsLF()
        {
            File.WriteAllText(_tempFile, "Hello\n");
            var fileModel = await _service.LoadAsync(_tempFile);

            Assert.AreEqual("Hello\n", fileModel.Content);
            Assert.AreEqual(LineEndings.LF, fileModel.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_EndsWithCR_DetectsCR()
        {
            File.WriteAllText(_tempFile, "Hello\r");
            var fileModel = await _service.LoadAsync(_tempFile);

            Assert.AreEqual("Hello\r", fileModel.Content);
            Assert.AreEqual(LineEndings.CR, fileModel.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_LFInMiddle_DetectsUnderscore()
        {
            // Legacy logic: LF only at end -> LF. If in middle and no CRLF -> _.
            File.WriteAllText(_tempFile, "Hello\nWorld");
            var fileModel = await _service.LoadAsync(_tempFile);

            Assert.AreEqual("Hello\nWorld", fileModel.Content);
            Assert.AreEqual(LineEndings._, fileModel.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_MixedCRLFAndLF_DetectsCRLF()
        {
            File.WriteAllText(_tempFile, "Hello\r\nWorld\n");
            var fileModel = await _service.LoadAsync(_tempFile);

            Assert.AreEqual("Hello\r\nWorld\n", fileModel.Content);
            Assert.AreEqual(LineEndings.CRLF, fileModel.LineEnding);
        }
    }
}
