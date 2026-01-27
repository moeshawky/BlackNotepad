using Microsoft.VisualStudio.TestTools.UnitTesting;
using Savaged.BlackNotepad.Models;
using Savaged.BlackNotepad.Services;
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
            _tempFile = Path.Combine(Path.GetTempPath(), "BlackNotepadTest_" + System.Guid.NewGuid().ToString() + ".txt");
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
        public async Task LoadAsync_EmptyFile_ReturnsFFFF()
        {
            // Arrange
            File.WriteAllText(_tempFile, string.Empty);
            var service = new FileModelService();

            // Act
            var result = await service.LoadAsync(_tempFile);

            // Assert
            Assert.AreEqual(((char)-1).ToString(), result.Content);
            Assert.AreEqual(LineEndings._, result.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_SingleLine_ReturnsContent()
        {
            // Arrange
            var content = "Hello World";
            File.WriteAllText(_tempFile, content);
            var service = new FileModelService();

            // Act
            var result = await service.LoadAsync(_tempFile);

            // Assert
            Assert.AreEqual(content, result.Content);
            Assert.AreEqual(LineEndings._, result.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_CRLF_Detected()
        {
            // Arrange
            var content = "Line1\r\nLine2";
            File.WriteAllText(_tempFile, content);
            var service = new FileModelService();

            // Act
            var result = await service.LoadAsync(_tempFile);

            // Assert
            Assert.AreEqual(content, result.Content);
            Assert.AreEqual(LineEndings.CRLF, result.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_LF_AtEnd_Detected()
        {
            // Arrange
            var content = "Line1\n";
            File.WriteAllText(_tempFile, content);
            var service = new FileModelService();

            // Act
            var result = await service.LoadAsync(_tempFile);

            // Assert
            Assert.AreEqual(content, result.Content);
            Assert.AreEqual(LineEndings.LF, result.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_LF_InMiddle_NotDetected()
        {
            // Arrange
            var content = "Line1\nLine2";
            File.WriteAllText(_tempFile, content);
            var service = new FileModelService();

            // Act
            var result = await service.LoadAsync(_tempFile);

            // Assert
            Assert.AreEqual(content, result.Content);
            Assert.AreEqual(LineEndings._, result.LineEnding);
        }

        [TestMethod]
        public async Task LoadAsync_CR_AtEnd_Detected()
        {
            // Arrange
            var content = "Line1\r";
            File.WriteAllText(_tempFile, content);
            var service = new FileModelService();

            // Act
            var result = await service.LoadAsync(_tempFile);

            // Assert
            Assert.AreEqual(content, result.Content);
            Assert.AreEqual(LineEndings.CR, result.LineEnding);
        }
    }
}
