using System;
using Savaged.BlackNotepad.Lookups;

namespace Savaged.BlackNotepad.Extensions
{
    public static class StringEx
    {
        public static int LineOfIndexOrDefault(
            this string self, int index, LineEndings lineEnding)
        {
            if (string.IsNullOrEmpty(self))
            {
                return 1;
            }

            var lineEndingStr = "\r";
            if (lineEnding == LineEndings.LF)
            {
                lineEndingStr = "\n";
            }
            else if (lineEnding == LineEndings.CRLF)
            {
                lineEndingStr = "\r\n";
            }

            var lines = self.Split(new[] { lineEndingStr }, 
                StringSplitOptions.None);

            var charCount = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                charCount += lines[i].Length;
                if (index < charCount)
                {
                    return i + 1;
                }
                charCount += lineEndingStr.Length;
            }

            return lines.Length > 0 ? lines.Length : 1;
        }
    }
}
