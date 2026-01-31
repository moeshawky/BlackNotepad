using Microsoft.VisualStudio.TestTools.UnitTesting;
using Savaged.BlackNotepad.Models;
using Savaged.BlackNotepad.Services;
using Savaged.BlackNotepad.Lookups;
using System.IO;
using System.Threading.Tasks;

namespace BlackNotepad.Test.UnitTests
{
    [TestClass]
    public class FileModelServiceTests
    {
        private string _tempFile;

        [TestInitialize]
        public void Setup()
        {
            _tempFile = Path.Combine(Path.GetTempPath(), "testfile_" + Path.GetRandomFileName());
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
        public async Task LoadAsync_ReadsCRLF()
        {
            File.WriteAllText(_tempFile, "Line1\r\nLine2");
            var service = new FileModelService();
            var model = await service.LoadAsync(_tempFile);

            Assert.AreEqual("Line1\r\nLine2", model.Content);
            Assert.AreEqual(LineEndings.CRLF, model.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_ReadsLFAtEnd()
        {
            File.WriteAllText(_tempFile, "Line1\n");
            var service = new FileModelService();
            var model = await service.LoadAsync(_tempFile);

            Assert.AreEqual("Line1\n", model.Content);
            Assert.AreEqual(LineEndings.LF, model.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_ReadsEmptyFile()
        {
            File.WriteAllText(_tempFile, "");
            var service = new FileModelService();
            var model = await service.LoadAsync(_tempFile);

            Assert.AreEqual(unchecked((char)-1).ToString(), model.Content);
            Assert.AreEqual(LineEndings._, model.LineEnding);
        }
    }
}
