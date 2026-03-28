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

            // Bolt: Optimized from char-by-char scan to string.IndexOf
            // Maintain exact bug-for-bug compatibility with legacy string iterations
            int linesCounted = 0;
            int currentIndex = 0;

            while (currentIndex <= index)
            {
                int nextIndex = self.IndexOf(lineEndingChar, currentIndex, index - currentIndex + 1);
                if (nextIndex == -1)
                {
                    break;
                }
                linesCounted++;
                currentIndex = nextIndex + 1;
            }

            if (index == self.Length - 1 && self[index] != lineEndingChar)
            {
                linesCounted++;
            }

            // Return linesCounted, preserving legacy logic where the first line
            // is effectively 0-indexed if newlines follow, but 1-indexed for a single-line string.
            return linesCounted;
        }
    }
}
