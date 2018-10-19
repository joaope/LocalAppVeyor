namespace LocalAppVeyor.Engine.IO
{
    public class FileInfo
    {
        private readonly System.IO.FileInfo _internalFileInfo;

        public FileInfo(string path)
        {
            _internalFileInfo = new System.IO.FileInfo(path);
        }

        public virtual string Name => _internalFileInfo.Name;

        public virtual FileInfo CopyTo(string destFileName, bool overwrite)
        {
            _internalFileInfo.CopyTo(destFileName, overwrite);

            var dest = new System.IO.FileInfo(destFileName);
            dest.Attributes &= ~System.IO.FileAttributes.ReadOnly;

            return new FileInfo(dest.FullName);
        }

        public virtual void Delete()
        {
            _internalFileInfo.Delete();
        }
    }
}