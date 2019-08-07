using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace LocalAppVeyor.Engine.UnitTests.TestUtilities
{
    internal sealed class WindowsOnlyFactAttribute : FactAttribute
    {
        public WindowsOnlyFactAttribute()
        {
            if (!MockUnixSupport.IsWindowsPlatform())
            {
                Skip = "Windows-only test";
            }
        }
    }
}