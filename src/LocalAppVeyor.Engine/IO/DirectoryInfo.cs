using System.Linq;

namespace LocalAppVeyor.Engine.IO
{
    public class DirectoryInfo
    {
        private readonly System.IO.DirectoryInfo internalDirectoryInfo;

        public virtual bool Exists => internalDirectoryInfo.Exists;

        public virtual string Name => internalDirectoryInfo.Name;

        public virtual string FullName => internalDirectoryInfo.FullName;

        public DirectoryInfo(string path)
        {
            internalDirectoryInfo = new System.IO.DirectoryInfo(path);
        }

        public virtual void Delete(bool recursive)
        {
            internalDirectoryInfo.Delete(recursive);
        }

        public virtual FileInfo[] GetFiles()
        {
            return internalDirectoryInfo
                .GetFiles()
                .Select(f => new FileInfo(f.FullName))
                .ToArray();
        }

        public virtual DirectoryInfo[] GetDirectories()
        {
            return internalDirectoryInfo
                .GetDirectories()
                .Select(d => new DirectoryInfo(d.FullName))
                .ToArray();
        }

        public virtual DirectoryInfo CreateSubdirectory(string path)
        {
            return new DirectoryInfo(internalDirectoryInfo.CreateSubdirectory(path).FullName);
        }
    }
}