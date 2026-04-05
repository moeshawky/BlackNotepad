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

            // Bolt: Optimized from char-by-char loop to IndexOf scan.
            int linesCounted = 0;
            int currentIndex = 0;

            while (true)
            {
                int nextNewline = self.IndexOf(lineEndingChar, currentIndex);

                if (nextNewline == -1 || nextNewline > index)
                {
                    // The index is before the next newline, or there are no more newlines
                    if (index == self.Length - 1 && self[index] != lineEndingChar)
                    {
                        linesCounted++; // Match legacy behavior for last char
                    }
                    return linesCounted;
                }

                linesCounted++;
                if (nextNewline == index) {
                    return linesCounted;
                }
                currentIndex = nextNewline + 1;
            }
        }
    }
}
