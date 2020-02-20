using System;

namespace FilenameChanger.Helpers
{
    public static class ConsoleHelpers
    {
        public static bool ConfirmChange(string confirmMessage = "Confirm Change")
        {
            Console.WriteLine();
            Console.WriteLine(confirmMessage + ": [Y] Yes, [N] No");
            return Console.ReadLine().ToLower() == "y";
        }

        public static void HighlightPatternInString(string str, string pattern, ConsoleColor color)
        {
            var containsPattern = true;
            var strCopy = str;

            do
            {
                var filenameBeforePattern = strCopy.Substring(0, strCopy.IndexOf(pattern));

                Console.Write(filenameBeforePattern);
                Console.ForegroundColor = color;
                Console.Write(pattern);
                Console.ForegroundColor = ConsoleColor.White;

                strCopy = strCopy.Substring(Math.Min(strCopy.IndexOf(pattern) + pattern.Length, strCopy.Length - 1));
                containsPattern = strCopy.Contains(pattern);
            } while (containsPattern);
        }
    }
}
