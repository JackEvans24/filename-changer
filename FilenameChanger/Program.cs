using NDesk.Options;
using System;
using System.Collections.Generic;
using System.IO;

namespace FilenameChanger
{
    class Program
    {
        static void Main(string[] args)
        {
            var showHelp = false;
            var directory = Environment.CurrentDirectory;
            var pattern = string.Empty;

            var p = new OptionSet() {
                { "d|directory=", "the {DIRECTORY} of files to change.",
                   v => directory = v },
                { "p|pattern=", "the string {PATTERN} to remove from file names.",
                   v => pattern = v },
                { "h|help",  "show this message and exit",
                   v => showHelp = v != null },
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("greet: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `greet --help' for more information.");
                return;
            }

            if (string.IsNullOrWhiteSpace(pattern))
            {
                Console.WriteLine("Directory or pattern not included");
                return;
            }

            Console.WriteLine("File changes:");
            Console.WriteLine();

            var files = Directory.GetFiles(directory);
            foreach (var file in files)
            {
                var fileObj = new FileInfo(file);
                var filename = fileObj.Name;
                Console.WriteLine("{0} => {1}", filename, filename.Replace(pattern, string.Empty));
            }

            Console.WriteLine();
            Console.WriteLine("Confirm change: [Y] Yes, [N] No");
            var confirm = Console.ReadLine().ToLower() == "y";

            if (confirm)
            {
                foreach (var file in files)
                {
                    var fileObj = new FileInfo(file);
                    if (!fileObj.Name.Contains(pattern))
                        continue;

                    File.Move(fileObj.FullName, $"{fileObj.DirectoryName}/{fileObj.Name.Replace(pattern, string.Empty).Trim()}");
                    File.Delete(fileObj.FullName);
                }
            }
        }
    }
}
