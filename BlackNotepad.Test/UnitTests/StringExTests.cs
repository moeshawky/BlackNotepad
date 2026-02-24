using Microsoft.VisualStudio.TestTools.UnitTesting;
using Savaged.BlackNotepad.Extensions;
using Savaged.BlackNotepad.Lookups;

namespace BlackNotepad.Test.UnitTests
{
    [TestClass]
    public class StringExTests
    {
        [TestMethod]
        public void LineOfIndexOrDefault_EmptyString()
        {
            var text = "";
            var result = text.LineOfIndexOrDefault(0, LineEndings.LF);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void LineOfIndexOrDefault_SingleLine_Start()
        {
            var text = "A";
            var result = text.LineOfIndexOrDefault(0, LineEndings.LF);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void LineOfIndexOrDefault_TwoLines_Start()
        {
            var text = "A\nB";
            var result = text.LineOfIndexOrDefault(0, LineEndings.LF);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void LineOfIndexOrDefault_TwoLines_Newline()
        {
            var text = "A\nB";
            var result = text.LineOfIndexOrDefault(1, LineEndings.LF);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void LineOfIndexOrDefault_TwoLines_End()
        {
            var text = "A\nB";
            var result = text.LineOfIndexOrDefault(2, LineEndings.LF);
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void LineOfIndexOrDefault_MultipleLines()
        {
            var text = "A\nB\nC";
            // Index 3 is second '\n'.
            // Matches: newline (1), newline (2). No last char logic unless index 4.
            // Wait, logic:
            // i=0. i=1 ('\n'). Count 1. i=2. i=3 ('\n'). Count 2. i=4 ('C'). i=Length-1. Count 3?
            // "A\nB\nC". Length 5.
            // Index 3 ('\n'). Newlines in [0..3]: at 1, 3. Count 2.
            // Index 4 ('C'). Newlines in [0..4]: at 1, 3. Count 2. Last char? Yes (index 4). Total 3.

            Assert.AreEqual(2, text.LineOfIndexOrDefault(3, LineEndings.LF));
            Assert.AreEqual(3, text.LineOfIndexOrDefault(4, LineEndings.LF));
        }

        [TestMethod]
        public void LineOfIndexOrDefault_OutOfBounds()
        {
            var text = "A";
            var result = text.LineOfIndexOrDefault(10, LineEndings.LF);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void LineOfIndexOrDefault_CRLF()
        {
            var text = "A\r\nB";
            // Uses \r as line ending char.
            // Index 0 ('A'). Result 0.
            // Index 1 ('\r'). Result 1.
            // Index 2 ('\n'). Result 1 (newline \n ignored unless last).
            // Index 3 ('B'). Last char. Result 2 (newline count 1 + 1).

            Assert.AreEqual(0, text.LineOfIndexOrDefault(0, LineEndings.CRLF));
            Assert.AreEqual(1, text.LineOfIndexOrDefault(1, LineEndings.CRLF));
            Assert.AreEqual(1, text.LineOfIndexOrDefault(2, LineEndings.CRLF));
            Assert.AreEqual(2, text.LineOfIndexOrDefault(3, LineEndings.CRLF));
        }
    }
}
