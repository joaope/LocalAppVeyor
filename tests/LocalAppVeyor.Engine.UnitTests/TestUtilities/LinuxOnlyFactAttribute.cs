using System.Runtime.InteropServices;
using Xunit;

namespace LocalAppVeyor.Engine.UnitTests.TestUtilities
{
    internal sealed class LinuxOnlyFactAttribute : FactAttribute
    {
        public LinuxOnlyFactAttribute()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Skip = "Linux-only test";
            }
        }
    }
}
