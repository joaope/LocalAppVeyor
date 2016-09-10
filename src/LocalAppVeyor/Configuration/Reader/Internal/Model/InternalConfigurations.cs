using System.Collections.Generic;

namespace LocalAppVeyor.Configuration.Reader.Internal.Model
{
    internal class InternalConfigurations : List<string>
    {
        public static implicit operator InternalConfigurations(string platform)
        {
            return new InternalConfigurations
            {
                platform
            };
        }
    }
}