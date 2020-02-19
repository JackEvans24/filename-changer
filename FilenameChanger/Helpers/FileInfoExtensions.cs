using System.IO;

namespace FilenameChanger.Helpers
{
    public static class FileInfoExtensions
    {
        public static string FilenameWithoutExtension(this FileInfo file)
        {
            return file.Name.Substring(0, file.Name.LastIndexOf('.'));
        }
    }
}
