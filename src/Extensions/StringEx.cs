using Savaged.BlackNotepad.Lookups;

namespace Savaged.BlackNotepad.Extensions
{
    public static class StringEx
    {
        public static int LineOfIndexOrDefault(
            this string self, int index, LineEndings lineEnding)
        {
            var lineEndingChar = '\r';
            if (lineEnding == LineEndings.LF)
            {
                lineEndingChar = '\n';
            }

            if (string.IsNullOrEmpty(self) || index < 0 || index >= self.Length)
            {
                return 1;
            }

            // Optimization: Use IndexOf to count newlines faster than char-by-char loop
            var count = 0;
            var pos = 0;

            while (pos < index)
            {
                int next = self.IndexOf(lineEndingChar, pos, index - pos);
                if (next < 0)
                {
                    break;
                }
                count++;
                pos = next + 1;
            }

            if (index == self.Length - 1 || self[index] == lineEndingChar)
            {
                count++;
            }

            return count;
        }
    }
}
