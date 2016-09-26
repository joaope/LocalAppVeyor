namespace LocalAppVeyor.Engine.IO
{
    public class FileInfo
    {
        private readonly System.IO.FileInfo internalFileInfo;

        public FileInfo(string path)
        {
            internalFileInfo = new System.IO.FileInfo(path);
        }

        public virtual string Name => internalFileInfo.Name;

        public virtual FileInfo CopyTo(string destFileName, bool overwrite)
        {
            internalFileInfo.CopyTo(destFileName, overwrite);

            var dest = new System.IO.FileInfo(destFileName);
            dest.Attributes &= ~System.IO.FileAttributes.ReadOnly;

            return new FileInfo(dest.FullName);
        }

        public virtual void Delete()
        {
            internalFileInfo.Delete();
        }
    }
}