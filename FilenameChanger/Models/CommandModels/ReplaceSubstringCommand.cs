using FilenameChanger.Helpers;
using ManyConsole;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilenameChanger.Models.CommandModels
{
    public class ReplaceSubstringCommand : ConsoleCommand
    {
        private string Pattern { get; set; }
        private string Token { get; set; }
        private string[] Extensions { get; set; }
        private string FileLocation { get; set; }

        public ReplaceSubstringCommand()
        {
            this.SetUpManyConsole();

            if (string.IsNullOrWhiteSpace(this.FileLocation))
                this.FileLocation = Environment.CurrentDirectory;
            if (string.IsNullOrEmpty(this.Token))
                this.Token = string.Empty;
        }

        private void SetUpManyConsole()
        {
            IsCommand("replace", "Replace a substring of all filenames in the directory with a replacement token");

            HasLongDescription("This can be used to replace the specified substring with a replacement string " +
                "for all files in the directory.\r\n" +
                "Useful when a downloaded set of files misspells a word or phrase, or for changing case of file names.");

            HasRequiredOption("p|pattern=", "The substring to replace", p => this.Pattern = p);

            HasOption("t|token=", "The token to replace with", t => this.Token = t);

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
                this.DisplayChanges(file.FilenameWithoutExtension());

            if (ConsoleHelpers.ConfirmChange())
            {
                foreach (var file in files)
                    this.RenameFile(file);
            }

            Console.WriteLine();
            Console.WriteLine("{0} changes made", files.Count());

            return 0;
        }

        private IEnumerable<FileInfo> GetValidFiles()
        {
            var files = Directory.GetFiles(this.FileLocation).Select(f => new FileInfo(f));

            if (Extensions != null)
                files = files.Where(f => Extensions.Contains(f.Extension));

            return files.Where(f => f.FilenameWithoutExtension().Contains(this.Pattern));
        }

        private int NoFiles()
        {
            Console.WriteLine($"No files found containing \"{this.Pattern}\" in directory: {this.FileLocation}");
            return 1;
        }

        private void DisplayChanges(string filename)
        {
            ConsoleHelpers.HighlightPatternInString(filename, this.Pattern, ConsoleColor.Red);
            Console.Write(" => ");

            var newFilename = filename.Replace(this.Pattern, this.Token);

            if (string.IsNullOrWhiteSpace(this.Token))
                Console.Write(newFilename);
            else
                ConsoleHelpers.HighlightPatternInString(newFilename, this.Token, ConsoleColor.Green);

            Console.WriteLine();
        }


        private void RenameFile(FileInfo file)
        {
            if (!file.FilenameWithoutExtension().Contains(this.Pattern))
                return;

            var newFilename = file.FilenameWithoutExtension().Replace(this.Pattern, this.Token).Trim() +
                file.Extension;
            newFilename = newFilename.Replace("  ", " ");

            File.Move(file.FullName, $"{file.DirectoryName}/{newFilename}");
            File.Delete(file.FullName);
        }
    }
}
