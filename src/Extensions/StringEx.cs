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

            var lineEndingChar = '\r';
            if (lineEnding == LineEndings.LF)
            {
                lineEndingChar = '\n';
            }

            // Optimized to use string.IndexOf instead of character-by-character scan.
            // This yields significant performance improvements for large strings.
            int linesCounted = 0;
            int currentPos = 0;

            while (currentPos <= index)
            {
                int nextMatch = self.IndexOf(lineEndingChar, currentPos);
                if (nextMatch != -1 && nextMatch <= index)
                {
                    linesCounted++;
                    currentPos = nextMatch + 1;
                }
                else
                {
                    break;
                }
            }

            // Legacy code quirk: explicitly increments linesCounted if the index
            // matches the last character of the string, and it's not a newline.
            if (index == self.Length - 1 && self[index] != lineEndingChar)
            {
                linesCounted++;
            }

            return linesCounted;
        }
    }
}
