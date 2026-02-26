using Savaged.BlackNotepad.Lookups;

namespace Savaged.BlackNotepad.Extensions
{
    public static class StringEx
    {
        public static int LineOfIndexOrDefault(
            this string self, int index, LineEndings lineEnding)
        {
            if (string.IsNullOrEmpty(self) || index < 0 || index >= self.Length)
            {
                return 1;
            }

            char lineEndingChar = (lineEnding == LineEndings.LF) ? '\n' : '\r';

            // Bolt: Optimized from O(N) loop to O(Lines) using IndexOf.
            // This is significantly faster for long files.
            // Logic must strictly match legacy behavior:
            // - Count newlines up to (and including) 'index'.
            // - If 'index' is the last character of the string, increment count (unless it's a newline).

            int count = 0;
            int pos = 0;

            // Count newlines in range [0, index]
            // We search for lineEndingChar in substring self.Substring(pos, length)
            // where length = index - pos + 1 (to include char at index)
            while ((pos = self.IndexOf(lineEndingChar, pos, index - pos + 1)) != -1)
            {
                count++;
                pos++;
            }

            // Legacy adjustment: if we are at the very end of the string,
            // and the last char is NOT a newline, it counts as a new line start?
            // "|| i == self.Length - 1" from original code.
            if (index == self.Length - 1 && self[index] != lineEndingChar)
            {
                count++;
            }

            return count;
        }
    }
}
