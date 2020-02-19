using FilenameChanger.Helpers;
using ManyConsole;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FilenameChanger.Models.CommandModels
{
    public class RemoveSubstringCommand : ConsoleCommand
    {
        public string Pattern { get; set; }
        public string[] Extensions { get; set; }
        public string FileLocation { get; set; }

        public RemoveSubstringCommand()
        {
            this.SetUpManyConsole();

            if (string.IsNullOrWhiteSpace(this.FileLocation))
                this.FileLocation = Environment.CurrentDirectory;
        }

        private void SetUpManyConsole()
        {
            IsCommand("remove", "Remove a substring of all filenames in the directory");

            HasLongDescription("This can be used to remove the specified substring from files in the directory.\r\n" +
                "Useful when a downloaded set of files includes a suffix you don't want.");

            HasRequiredOption("p|pattern=", "The substring to remove from files", p => this.Pattern = p);

            HasOption("e|ext=", "A comma separated list of file types to alter. If not provided, all files are altered",
                e => this.Extensions = e.Split(','));

            HasOption("f|file=", "The full path of the file. Defaults to the current console location",
                f => this.FileLocation = f);
        }

        public override int Run(string[] remainingArguments)
        {
            var files = this.GetValidFiles();

            if (!files.Any())
                return NoFiles();

            foreach (var file in files)
            {
                var filename = file.FilenameWithoutExtension();
                Console.WriteLine("{0} => {1}", filename, filename.Replace(this.Pattern, string.Empty));
            }

            if (ConsoleHelpers.ConfirmChange())
            {
                foreach (var file in files)
                {
                    this.RenameFile(file);
                }
            }

            return 0;
        }

        private IEnumerable<FileInfo> GetValidFiles()
        {
            var files = Directory.GetFiles(this.FileLocation).Select(f => new FileInfo(f));

            if (Extensions != null)
                files = files.Where(f => Extensions.Contains(f.Extension));

            return files;
        }

        private int NoFiles()
        {
            Console.WriteLine($"No files found in directory: {this.FileLocation}");
            return 1;
        }

        private void RenameFile(FileInfo file)
        {
            if (!file.FilenameWithoutExtension().Contains(this.Pattern))
                return;

            var newFilename = file.FilenameWithoutExtension().Replace(this.Pattern, string.Empty).Trim() +
                file.Extension;
            newFilename = newFilename.Replace("  ", " ");

            File.Move(file.FullName, $"{file.DirectoryName}/{newFilename}");
            File.Delete(file.FullName);
        }
    }
}
