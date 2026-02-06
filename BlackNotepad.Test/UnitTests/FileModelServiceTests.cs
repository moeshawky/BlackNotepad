using Microsoft.VisualStudio.TestTools.UnitTesting;
using Savaged.BlackNotepad.Lookups;
using Savaged.BlackNotepad.Models;
using Savaged.BlackNotepad.Services;
using System.IO;

namespace Savaged.BlackNotepad.Test.UnitTests
{
    [TestClass]
    public class FileModelServiceTests
    {
        private FileModelService _service;

        [TestInitialize]
        public void Init()
        {
            _service = new FileModelService();
        }

        [TestMethod]
        public void ReadFile_EmptyFile_ReturnsFFFF()
        {
            string tempFile = Path.GetTempFileName();
            // GetTempFileName creates a zero-byte file on disk.

            try
            {
                var task = _service.LoadAsync(tempFile);
                task.Wait();
                var result = task.Result;

                Assert.AreEqual("\uffff", result.Content, "Empty file should return \\uffff as content per legacy behavior");
                Assert.AreEqual(LineEndings._, result.LineEnding);
            }
            finally
            {
                if (File.Exists(tempFile)) File.Delete(tempFile);
            }
        }

        [TestMethod]
        public void ReadFile_CRLF()
        {
            VerifyLineEnding("A\r\nB", LineEndings.CRLF);
        }

        [TestMethod]
        public void ReadFile_LF_AtEnd()
        {
            VerifyLineEnding("A\n", LineEndings.LF);
        }

        [TestMethod]
        public void ReadFile_CR_AtEnd()
        {
             VerifyLineEnding("A\r", LineEndings.CR);
        }

        [TestMethod]
        public void ReadFile_LF_InMiddle_ReturnsUnknown()
        {
            VerifyLineEnding("A\nB", LineEndings._);
        }

        [TestMethod]
        public void ReadFile_JustLF_AtEnd()
        {
            VerifyLineEnding("\n", LineEndings.LF);
        }

        private void VerifyLineEnding(string content, LineEndings expectedEnding)
        {
            string tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, content);
            try
            {
                var task = _service.LoadAsync(tempFile);
                task.Wait();
                var result = task.Result;

                Assert.AreEqual(content, result.Content);
                Assert.AreEqual(expectedEnding, result.LineEnding, $"Failed for content: {content.Replace("\r", "\\r").Replace("\n", "\\n")}");
            }
            finally
            {
                if (File.Exists(tempFile)) File.Delete(tempFile);
            }
        }
    }
}
