namespace LocalAppVeyor.Engine.IO
{
    public class PathHandler
    {
        public virtual string Combine(params string[] paths)
        {
            return System.IO.Path.Combine(paths);
        }

        public virtual string GetTempPath()
        {
            return System.IO.Path.GetTempPath();
        }

        public static PathHandler Instance { get; } = new PathHandler();
    }
}