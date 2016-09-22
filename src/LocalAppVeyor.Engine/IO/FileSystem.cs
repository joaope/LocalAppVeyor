namespace LocalAppVeyor.Engine.IO
{
    public class FileSystem
    {
        public virtual DirectoryHandler Directory { get; private set; }

        public virtual FileHandler File { get; private set; }

        public virtual PathHandler Path { get; private set; }

        public FileSystem(DirectoryHandler directory, FileHandler file, PathHandler path)
        {
            Directory = directory;
            File = file;
            Path = path;
        }

        public FileSystem()
            : this(DirectoryHandler.Instance, FileHandler.Instance, PathHandler.Instance)
        {
        }

        public static FileSystem Instance { get; } = new FileSystem();
    }
}