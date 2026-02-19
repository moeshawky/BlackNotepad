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
        private string _tempFile;

        [TestCleanup]
        public void Cleanup()
        {
            if (!string.IsNullOrEmpty(_tempFile) && File.Exists(_tempFile))
            {
                File.Delete(_tempFile);
            }
        }

        [TestMethod]
        public async Task TestLoadAsync_DetectsLineEndings_LF()
        {
            _tempFile = "test_lf.txt";
            File.WriteAllText(_tempFile, "Line1\nLine2");

            var service = new FileModelService();
            var model = await service.LoadAsync(_tempFile);

            Assert.AreEqual("Line1\nLine2", model.Content);
            Assert.AreEqual(LineEndings.LF, model.LineEnding);
        }

        [TestMethod]
        public async Task TestLoadAsync_DetectsLineEndings_CRLF()
        {
            _tempFile = "test_crlf.txt";
            File.WriteAllText(_tempFile, "Line1\r\nLine2");

            var service = new FileModelService();
            var model = await service.LoadAsync(_tempFile);

            Assert.AreEqual("Line1\r\nLine2", model.Content);
            Assert.AreEqual(LineEndings.CRLF, model.LineEnding);
        }

        [TestMethod]
        public async Task TestLoadAsync_DetectsLineEndings_CR()
        {
            _tempFile = "test_cr.txt";
            File.WriteAllText(_tempFile, "Line1\rLine2");

            var service = new FileModelService();
            var model = await service.LoadAsync(_tempFile);

            Assert.AreEqual("Line1\rLine2", model.Content);
            Assert.AreEqual(LineEndings.CR, model.LineEnding);
        }

        [TestMethod]
        public async Task TestLoadAsync_EmptyFile()
        {
            _tempFile = "test_empty.txt";
            File.WriteAllText(_tempFile, "");

            var service = new FileModelService();
            var model = await service.LoadAsync(_tempFile);

            Assert.AreEqual("", model.Content);
            Assert.AreEqual(LineEndings._, model.LineEnding);
        }

        [TestMethod]
        public async Task TestLoadAsync_MixedEndings_FirstTakesPrecedence()
        {
             // Test that the first line ending determines the detected type
             _tempFile = "test_mixed.txt";
             // LF first
             File.WriteAllText(_tempFile, "Line1\nLine2\r\n");

             var service = new FileModelService();
             var model = await service.LoadAsync(_tempFile);

             Assert.AreEqual(LineEndings.LF, model.LineEnding);
        }
    }
}
