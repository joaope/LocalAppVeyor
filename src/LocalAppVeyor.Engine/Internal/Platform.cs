using System.IO;

namespace LocalAppVeyor.Engine.Internal
{
    public static class Platform
    {
        public static bool IsWindow { get; }

        public static bool IsUnix { get; }

        static Platform()
        {
            IsUnix = Path.DirectorySeparatorChar == '/';
            IsWindow = !IsUnix;
        }
    }
}
