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
            return new FileInfo(destFileName);
        }

        public virtual void Delete()
        {
            internalFileInfo.Delete();
        }
    }
}