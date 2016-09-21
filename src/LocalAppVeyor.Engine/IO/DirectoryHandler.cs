using System.Collections.Generic;

namespace LocalAppVeyor.Engine.IO
{
    public class DirectoryHandler
    {
        public virtual bool Exists(string path)
        {
            return System.IO.Directory.Exists(path);
        }

        public virtual DirectoryInfo CreateDirectory(string path)
        {
            return new DirectoryInfo(System.IO.Directory.CreateDirectory(path).FullName);
        }

        public virtual IEnumerable<string> EnumerateFiles(string searchPattern)
        {
            return System.IO.Directory.EnumerateFiles(searchPattern);
        }

        public virtual IEnumerable<string> EnumerateFiles(string path, string searchPattern)
        {
            return System.IO.Directory.EnumerateFiles(path, searchPattern);
        }

        public virtual IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
        {
            return System.IO.Directory.EnumerateFiles(
                path,
                searchPattern,
                searchOption == SearchOption.AllDirectories
                    ? System.IO.SearchOption.AllDirectories
                    : System.IO.SearchOption.TopDirectoryOnly);
        }
    }
}