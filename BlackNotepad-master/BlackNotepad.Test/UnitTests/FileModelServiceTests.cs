using Microsoft.VisualStudio.TestTools.UnitTesting;
using Savaged.BlackNotepad.Lookups;
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
            _tempFile = Path.GetTempFileName();
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
        public async Task LoadAsync_EmptyFile_ReturnsEmptyContentAndDefaultLineEnding()
        {
            File.WriteAllText(_tempFile, "");
            var result = await _service.LoadAsync(_tempFile);

            Assert.AreEqual("", result.Content);
            Assert.AreEqual(LineEndings._, result.LineEnding);
            Assert.IsFalse(result.IsDirty);
        }

        [TestMethod]
        public async Task LoadAsync_SimpleText_ReturnsContentAndDefaultLineEnding()
        {
            var content = "Hello World";
            File.WriteAllText(_tempFile, content);
            var result = await _service.LoadAsync(_tempFile);

            Assert.AreEqual(content, result.Content);
            Assert.AreEqual(LineEndings._, result.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_CRLF_Detected()
        {
            var content = "Line1\r\nLine2";
            File.WriteAllText(_tempFile, content);
            var result = await _service.LoadAsync(_tempFile);

            Assert.AreEqual(content, result.Content);
            Assert.AreEqual(LineEndings.CRLF, result.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_LF_DetectedAtEnd()
        {
            var content = "Line1\n";
            File.WriteAllText(_tempFile, content);
            var result = await _service.LoadAsync(_tempFile);

            Assert.AreEqual(content, result.Content);
            Assert.AreEqual(LineEndings.LF, result.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_CR_DetectedAtEnd()
        {
            var content = "Line1\r";
            File.WriteAllText(_tempFile, content);
            var result = await _service.LoadAsync(_tempFile);

            Assert.AreEqual(content, result.Content);
            Assert.AreEqual(LineEndings.CR, result.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_LF_NotDetectedIfNotAtEnd()
        {
            var content = "Line1\nLine2";
            File.WriteAllText(_tempFile, content);
            var result = await _service.LoadAsync(_tempFile);

            Assert.AreEqual(content, result.Content);
            Assert.AreEqual(LineEndings._, result.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_Mixed_CRLF_Priority()
        {
            var content = "Line1\r\nLine2\n";
            File.WriteAllText(_tempFile, content);
            var result = await _service.LoadAsync(_tempFile);

            Assert.AreEqual(content, result.Content);
            Assert.AreEqual(LineEndings.CRLF, result.LineEnding);
        }
    }
}
