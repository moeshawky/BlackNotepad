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

            // Counts separator occurrences at or before index instead of
            // splitting the document. Proven equivalent to the prior
            // Split-based walk: Split returns i + 1 at the first line whose
            // end offset exceeds index, which is exactly 1 plus the number
            // of separators starting at or before index. O(n) time, O(1)
            // allocations instead of O(n) substring copies.
            var line = 1;
            var position = self.IndexOf(lineEndingStr, StringComparison.Ordinal);
            while (position >= 0 && position <= index)
            {
                line++;
                position = self.IndexOf(
                    lineEndingStr,
                    position + lineEndingStr.Length,
                    StringComparison.Ordinal);
            }

            return line;
        }
    }
}
