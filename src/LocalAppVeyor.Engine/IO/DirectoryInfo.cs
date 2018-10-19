using System.Linq;

namespace LocalAppVeyor.Engine.IO
{
    public class DirectoryInfo
    {
        private readonly System.IO.DirectoryInfo _internalDirectoryInfo;

        public virtual bool Exists => _internalDirectoryInfo.Exists;

        public virtual string Name => _internalDirectoryInfo.Name;

        public virtual string FullName => _internalDirectoryInfo.FullName;

        public DirectoryInfo(string path)
        {
            _internalDirectoryInfo = new System.IO.DirectoryInfo(path);
        }

        public virtual void Delete(bool recursive)
        {
            _internalDirectoryInfo.Delete(recursive);
        }

        public virtual FileInfo[] GetFiles()
        {
            return _internalDirectoryInfo
                .GetFiles()
                .Select(f => new FileInfo(f.FullName))
                .ToArray();
        }

        public virtual DirectoryInfo[] GetDirectories()
        {
            return _internalDirectoryInfo
                .GetDirectories()
                .Select(d => new DirectoryInfo(d.FullName))
                .ToArray();
        }

        public virtual DirectoryInfo CreateSubdirectory(string path)
        {
            return new DirectoryInfo(_internalDirectoryInfo.CreateSubdirectory(path).FullName);
        }
    }
}