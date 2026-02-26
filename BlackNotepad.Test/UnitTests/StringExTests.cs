using Microsoft.VisualStudio.TestTools.UnitTesting;
using Savaged.BlackNotepad.Extensions;
using Savaged.BlackNotepad.Lookups;

namespace BlackNotepad.Test.UnitTests
{
    [TestClass]
    public class StringExTests
    {
        [TestMethod]
        public void LineOfIndexOrDefault_ShouldReturnCorrectLineNumber()
        {
            // Case 1: Simple single line
            // "A" -> Line 1
            Assert.AreEqual(1, "A".LineOfIndexOrDefault(0, LineEndings.LF));

            // Case 2: Simple multi-line
            // "A\nB"
            // i=0 ('A') -> 0
            // i=1 ('\n') -> 1
            // i=2 ('B') -> 2
            Assert.AreEqual(0, "A\nB".LineOfIndexOrDefault(0, LineEndings.LF));
            Assert.AreEqual(1, "A\nB".LineOfIndexOrDefault(1, LineEndings.LF));
            Assert.AreEqual(2, "A\nB".LineOfIndexOrDefault(2, LineEndings.LF));

            // Case 3: Empty string
            // returns 1
            Assert.AreEqual(1, "".LineOfIndexOrDefault(0, LineEndings.LF));

            // Case 4: Out of bounds
            Assert.AreEqual(1, "A".LineOfIndexOrDefault(1, LineEndings.LF));
        }

        [TestMethod]
        public void LineOfIndexOrDefault_ShouldHandleCRLF()
        {
             // "A\r\nB"
             // Using CRLF means we check for \r as line ending
             string text = "A\r\nB";

             // i=0 ('A') -> 0 (Not \r, not last)
             Assert.AreEqual(0, text.LineOfIndexOrDefault(0, LineEndings.CRLF));

             // i=1 ('\r') -> 1 (Is \r)
             Assert.AreEqual(1, text.LineOfIndexOrDefault(1, LineEndings.CRLF));

             // i=2 ('\n') -> 1 (Not \r, not last)
             Assert.AreEqual(1, text.LineOfIndexOrDefault(2, LineEndings.CRLF));

             // i=3 ('B') -> 2 (Is last)
             Assert.AreEqual(2, text.LineOfIndexOrDefault(3, LineEndings.CRLF));
        }

        [TestMethod]
        public void LineOfIndexOrDefault_ShouldHandleMixedEndings()
        {
            // "A\nB\rC"
            // With LF: checks \n
            string text = "A\nB\rC";
            // i=0 ('A') -> 0
            Assert.AreEqual(0, text.LineOfIndexOrDefault(0, LineEndings.LF));
            // i=1 ('\n') -> 1
            Assert.AreEqual(1, text.LineOfIndexOrDefault(1, LineEndings.LF));
            // i=2 ('B') -> 1
            Assert.AreEqual(1, text.LineOfIndexOrDefault(2, LineEndings.LF));
            // i=3 ('\r') -> 1
            Assert.AreEqual(1, text.LineOfIndexOrDefault(3, LineEndings.LF));
            // i=4 ('C') -> 2 (last)
            Assert.AreEqual(2, text.LineOfIndexOrDefault(4, LineEndings.LF));
        }
    }
}
