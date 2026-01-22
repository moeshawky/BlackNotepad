using Microsoft.VisualStudio.TestTools.UnitTesting;
using Savaged.BlackNotepad.Lookups;
using Savaged.BlackNotepad.Models;
using Savaged.BlackNotepad.Services;
using System.IO;
using System.Threading.Tasks;

namespace BlackNotepad.Test.Services
{
    [TestClass]
    public class FileModelServiceTests
    {
        private string _tempFile;

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_tempFile)) File.Delete(_tempFile);
        }

        private async Task CreateTempFile(string content)
        {
            _tempFile = Path.GetTempFileName();
            using (var sw = new StreamWriter(_tempFile))
            {
                await sw.WriteAsync(content);
            }
        }

        [TestMethod]
        public async Task LoadAsync_GivenEmptyFile_ReturnsEmptyContent()
        {
            await CreateTempFile(string.Empty);

            var service = new FileModelService();
            var result = await service.LoadAsync(_tempFile);

            Assert.AreEqual(string.Empty, result.Content);
            Assert.AreEqual(LineEndings._, result.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_GivenSimpleText_ReturnsContentAndDefaultLineEnding()
        {
            var content = "Hello World";
            await CreateTempFile(content);

            var service = new FileModelService();
            var result = await service.LoadAsync(_tempFile);

            Assert.AreEqual(content, result.Content);
            Assert.AreEqual(LineEndings._, result.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_GivenCRLF_ReturnsCRLF()
        {
            var content = "Hello\r\nWorld";
            await CreateTempFile(content);

            var service = new FileModelService();
            var result = await service.LoadAsync(_tempFile);

            Assert.AreEqual(content, result.Content);
            Assert.AreEqual(LineEndings.CRLF, result.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_GivenLFAtEnd_ReturnsLF()
        {
            var content = "Hello\n";
            await CreateTempFile(content);

            var service = new FileModelService();
            var result = await service.LoadAsync(_tempFile);

            Assert.AreEqual(content, result.Content);
            Assert.AreEqual(LineEndings.LF, result.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_GivenCRAtEnd_ReturnsCR()
        {
            var content = "Hello\r";
            await CreateTempFile(content);

            var service = new FileModelService();
            var result = await service.LoadAsync(_tempFile);

            Assert.AreEqual(content, result.Content);
            Assert.AreEqual(LineEndings.CR, result.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_GivenMixedLineEndings_PrioritizesCRLF()
        {
            var content = "Hello\nWorld\r\n";
            await CreateTempFile(content);

            var service = new FileModelService();
            var result = await service.LoadAsync(_tempFile);

            Assert.AreEqual(content, result.Content);
            Assert.AreEqual(LineEndings.CRLF, result.LineEnding);
        }
    }
}
