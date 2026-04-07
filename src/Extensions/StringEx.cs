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
            int linesCounted = 0;
            int currentIndex = -1;

            while (true)
            {
                int nextIndex = self.IndexOf(lineEndingChar, currentIndex + 1);

                if (nextIndex == -1 || nextIndex >= index)
                {
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

                linesCounted++;
                currentIndex = nextIndex;
            }
        }
    }
}
