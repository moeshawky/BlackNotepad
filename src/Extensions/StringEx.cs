using Savaged.BlackNotepad.Lookups;

namespace Savaged.BlackNotepad.Extensions
{
    public static class StringEx
    {
        public static int LineOfIndexOrDefault(
            this string self, int index, LineEndings lineEnding)
        {
            // Default value is 1 for empty string or out of bounds, matching legacy behavior.
            if (string.IsNullOrEmpty(self) || index < 0 || index >= self.Length)
            {
                return 1;
            }

            var lineEndingChar = '\r';
            if (lineEnding == LineEndings.LF)
            {
                lineEndingChar = '\n';
            }
            var linesCounted = 0;
            var value = 1;

            int currentPos = 0;
            int length = self.Length;

            // Bolt: Optimized from O(N) char-by-char loop to O(Lines) using IndexOf.
            while (currentPos < length)
            {
                // Find next newline
                int nextNewline = self.IndexOf(lineEndingChar, currentPos);

                // Case 1: No more newlines
                if (nextNewline == -1)
                {
                    // If index is in this last segment [currentPos, length-1]
                    // If index == length - 1, we increment because loop would have hit i == self.Length - 1
                    if (index == length - 1)
                    {
                        value = linesCounted + 1;
                    }
                    else if (index >= currentPos)
                    {
                        // index is in valid range but not the last char
                        value = linesCounted;
                    }
                    break;
                }

                // Case 2: Newline found strictly after index
                if (nextNewline > index)
                {
                    // index is before the newline.
                    // linesCounted not incremented yet.
                    value = linesCounted;
                    break;
                }

                // Case 3: Newline is at or before index
                // We consume this newline.
                linesCounted++;

                if (nextNewline == index)
                {
                    value = linesCounted;
                    break;
                }

                currentPos = nextNewline + 1;
            }

            return value;
        }
    }
}
