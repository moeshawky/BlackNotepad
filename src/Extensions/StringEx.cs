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

            char lineEndingChar = lineEnding == LineEndings.LF ? '\n' : '\r';
            int linesCounted = 0;
            int currentIndex = 0;

            while (currentIndex <= index)
            {
                int nextNewline = self.IndexOf(lineEndingChar, currentIndex, index - currentIndex + 1);
                if (nextNewline == -1)
                {
                    break;
                }

                linesCounted++;
                currentIndex = nextNewline + 1;
            }

            if (index == self.Length - 1 && self[index] != lineEndingChar)
            {
                linesCounted++;
            }

            return linesCounted;
        }
    }
}