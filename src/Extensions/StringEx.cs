using Savaged.BlackNotepad.Lookups;

namespace Savaged.BlackNotepad.Extensions
{
    public static class StringEx
    {
        public static int LineOfIndexOrDefault(
            this string self, int index, LineEndings lineEnding)
        {
            // Bolt: Optimized to use IndexOf loop for performance (O(Lines) vs O(N)).
            // Returns 1 if string is empty or index out of bounds to match original behavior.
            if (string.IsNullOrEmpty(self)) return 1;
            if (index >= self.Length) return 1;

            var lineEndingChar = '\r';
            if (lineEnding == LineEndings.LF)
            {
                lineEndingChar = '\n';
            }

            int linesCounted = 0;
            int currentPos = 0;

            while (true)
            {
                int nextNewline = self.IndexOf(lineEndingChar, currentPos);

                // If no more newlines, or next newline is beyond index
                if (nextNewline == -1 || nextNewline > index)
                {
                    // Check if index reaches the end of string.
                    // If we are at the last character and it's NOT a newline,
                    // we count it as a line ending (matching original behavior).
                    if (index == self.Length - 1 && self[index] != lineEndingChar)
                    {
                        linesCounted++;
                    }
                    return linesCounted;
                }

                // Found a newline at `nextNewline` <= index.
                linesCounted++;
                currentPos = nextNewline + 1;
            }
        }
    }
}
