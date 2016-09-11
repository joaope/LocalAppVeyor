using System.Collections.Generic;

namespace LocalAppVeyor.Engine.Configuration.Reader.Internal.Model
{
    internal class InternalPlatforms : List<string>
    {
        public static implicit operator InternalPlatforms(string platform)
        {
            return new InternalPlatforms
            {
                platform
            };
        }
    }
}
