namespace LocalAppVeyor.Engine.IO
{
    public class FileSystem
    {
        public virtual DirectoryHandler Directory { get; private set; }

        public virtual FileHandler File { get; private set; }

        public virtual PathHandler Path { get; private set; }

        public FileSystem()
        {
            Directory = new DirectoryHandler();
            File = new FileHandler();
            Path = new PathHandler();
        }

        public static FileSystem Instance { get; } = new FileSystem();
    }
}