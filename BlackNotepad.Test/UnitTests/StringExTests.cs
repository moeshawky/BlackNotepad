using Microsoft.VisualStudio.TestTools.UnitTesting;
using Savaged.BlackNotepad.Extensions;
using Savaged.BlackNotepad.Lookups;

namespace BlackNotepad.Test.UnitTests
{
    [TestClass]
    public class StringExTests
    {
        [TestMethod]
        public void TestLineOfIndexOrDefault_Empty()
        {
            var text = string.Empty;
            var result = text.LineOfIndexOrDefault(0, LineEndings.LF);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestLineOfIndexOrDefault_SingleChar()
        {
            var text = "A";
            var result = text.LineOfIndexOrDefault(0, LineEndings.LF);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestLineOfIndexOrDefault_TwoChars()
        {
            var text = "AB";
            var result = text.LineOfIndexOrDefault(0, LineEndings.LF);
            Assert.AreEqual(0, result);
            result = text.LineOfIndexOrDefault(1, LineEndings.LF);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestLineOfIndexOrDefault_NewlineLF()
        {
            var text = "A\nB";
            var result = text.LineOfIndexOrDefault(0, LineEndings.LF);
            Assert.AreEqual(0, result);
            result = text.LineOfIndexOrDefault(1, LineEndings.LF);
            Assert.AreEqual(1, result);
            result = text.LineOfIndexOrDefault(2, LineEndings.LF);
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void TestLineOfIndexOrDefault_OnlyNewlineLF()
        {
            var text = "A\n";
            var result = text.LineOfIndexOrDefault(0, LineEndings.LF);
            Assert.AreEqual(0, result);
            result = text.LineOfIndexOrDefault(1, LineEndings.LF);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestLineOfIndexOrDefault_MixedCRLF()
        {
            var text = "A\r\n";
            var result = text.LineOfIndexOrDefault(0, LineEndings.CRLF);
            Assert.AreEqual(0, result);
            result = text.LineOfIndexOrDefault(1, LineEndings.CRLF);
            Assert.AreEqual(1, result);
            result = text.LineOfIndexOrDefault(2, LineEndings.CRLF);
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void TestLineOfIndexOrDefault_OutOfBounds()
        {
            var text = "A";
            var result = text.LineOfIndexOrDefault(10, LineEndings.LF);
            Assert.AreEqual(1, result);
        }
    }
}
