namespace LocalAppVeyor.Engine.IO
{
    public class FileHandler
    {
        public virtual string ReadAllText(string path)
        {
            return System.IO.File.ReadAllText(path);
        }

        public virtual void WriteAllText(string path, string contents)
        {
            System.IO.File.WriteAllText(path, contents);
        }

        public virtual bool Exists(string path)
        {
            return System.IO.File.Exists(path);
        }
    }
}