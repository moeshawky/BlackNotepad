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

            var lineEndingChar = lineEnding == LineEndings.LF ? '\n' : '\r';
            var linesCounted = 0;
            var currentIndex = 0;

            // Optimize by jumping between newlines instead of scanning character-by-character
            while (true)
            {
                int nextIndex = self.IndexOf(lineEndingChar, currentIndex);

                if (nextIndex == -1 || nextIndex > index)
                {
                    break;
                }

                linesCounted++;
                currentIndex = nextIndex + 1;
            }

            // Maintain legacy behavior: increment line count if index is at the very end
            // of the string and the last character is not a newline.
            if (index == self.Length - 1 && self[index] != lineEndingChar)
            {
                linesCounted++;
            }

            return linesCounted;
        }
    }
}
