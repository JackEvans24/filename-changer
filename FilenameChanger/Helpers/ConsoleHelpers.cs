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
    }
}
