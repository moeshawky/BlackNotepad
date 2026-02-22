using Savaged.BlackNotepad.Lookups;

namespace Savaged.BlackNotepad.Extensions
{
    public static class StringEx
    {
        public static int LineOfIndexOrDefault(
            this string self, int index, LineEndings lineEnding)
        {
            // Bolt: Optimization - early return for empty string or out-of-bounds index
            // to match the original implementation's default return value of 1.
            if (string.IsNullOrEmpty(self) || index >= self.Length)
            {
                return 1;
            }

            var lineEndingChar = '\r';
            if (lineEnding == LineEndings.LF)
            {
                lineEndingChar = '\n';
            }

            var linesCounted = 0;
            var pos = 0;

            // Bolt: Optimization - replace O(N) character iteration with O(Lines) IndexOf loop.
            // This avoids iterating through every character in the string, resulting in
            // significantly better performance for large files (O(L) vs O(N)).
            while ((pos = self.IndexOf(lineEndingChar, pos)) != -1)
            {
                if (pos > index)
                {
                    break;
                }
                linesCounted++;
                pos++;
            }

            // Bolt: Preserve original logic where the last line is counted if the index
            // falls on the last character and it's not a newline.
            if (index == self.Length - 1 && self[index] != lineEndingChar)
            {
                linesCounted++;
            }

            return linesCounted;
        }
    }
}
