using Savaged.BlackNotepad.Lookups;

namespace Savaged.BlackNotepad.Extensions
{
    public static class StringEx
    {
        public static int LineOfIndexOrDefault(
            this string self, int index, LineEndings lineEnding)
        {
            // Bolt: Optimized line counting by replacing character-by-character scan
            // with string.IndexOf jumps, significantly reducing overhead for large strings.
            if (string.IsNullOrEmpty(self) || index < 0 || index >= self.Length)
            {
                return 1;
            }

            var lineEndingChar = '\r';
            if (lineEnding == LineEndings.LF)
            {
                lineEndingChar = '\n';
            }

            int linesCounted = 0;
            int searchStart = 0;

            while (true)
            {
                int nextNewline = self.IndexOf(lineEndingChar, searchStart);
                if (nextNewline == -1 || nextNewline >= index)
                {
                    break;
                }
                linesCounted++;
                searchStart = nextNewline + 1;
            }

            if (index == self.Length - 1 && self[index] != lineEndingChar)
            {
                linesCounted++;
            }
            else if (self[index] == lineEndingChar)
            {
                linesCounted++;
            }

            return linesCounted;
        }
    }
}
