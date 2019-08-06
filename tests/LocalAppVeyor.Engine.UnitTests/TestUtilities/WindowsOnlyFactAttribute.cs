using System.Runtime.InteropServices;
using Xunit;

namespace LocalAppVeyor.Engine.UnitTests.TestUtilities
{
    internal sealed class WindowsOnlyFactAttribute : FactAttribute
    {
        public WindowsOnlyFactAttribute()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Skip = "Windows-only test";
            }
        }
    }
}