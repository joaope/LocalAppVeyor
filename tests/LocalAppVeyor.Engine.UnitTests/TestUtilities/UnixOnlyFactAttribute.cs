using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace LocalAppVeyor.Engine.UnitTests.TestUtilities
{
    internal sealed class UnixOnlyFactAttribute : FactAttribute
    {
        public UnixOnlyFactAttribute()
        {
            if (!MockUnixSupport.IsUnixPlatform())
            {
                Skip = "Unix-only test";
            }
        }
    }
}
