using System.IO.Abstractions;

namespace LocalAppVeyor.Engine.Internal.Extensions
{
    internal static class FileSystemExtensions
    {
        public static string GetTemporaryDirectory(this IFileSystem fileSystem)
        {
            string tempDirectory;

            do
            {
                tempDirectory = fileSystem.Path.Combine(fileSystem.Path.GetTempPath(), fileSystem.Path.GetRandomFileName());
            } while (fileSystem.Directory.Exists(tempDirectory));

            fileSystem.Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
    }
}
