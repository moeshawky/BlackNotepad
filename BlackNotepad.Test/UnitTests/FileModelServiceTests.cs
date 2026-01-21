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
        [TestMethod]
        public async Task TestLoadAsync_CRLF()
        {
            var service = new FileModelService();
            var content = "Line1\r\nLine2";
            var path = Path.GetTempFileName();
            File.WriteAllText(path, content);
            try
            {
                var model = await service.LoadAsync(path);
                Assert.AreEqual(content, model.Content);
                Assert.AreEqual(LineEndings.CRLF, model.LineEnding);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [TestMethod]
        public async Task TestLoadAsync_LF()
        {
            var service = new FileModelService();
            var content = "Line1\n";
            var path = Path.GetTempFileName();
            File.WriteAllText(path, content);
            try
            {
                var model = await service.LoadAsync(path);
                Assert.AreEqual(content, model.Content);
                Assert.AreEqual(LineEndings.LF, model.LineEnding);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [TestMethod]
        public async Task TestLoadAsync_Empty()
        {
            var service = new FileModelService();
            var content = "";
            var path = Path.GetTempFileName();
            File.WriteAllText(path, content);
            try
            {
                var model = await service.LoadAsync(path);
                Assert.AreEqual(string.Empty, model.Content);
                Assert.AreEqual(LineEndings._, model.LineEnding);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }
    }
}
