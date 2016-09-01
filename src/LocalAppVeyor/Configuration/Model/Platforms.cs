using System.Collections.Generic;

namespace LocalAppVeyor.Configuration.Model
{
    public class Platforms : List<string>
    {
        public static implicit operator Platforms(string platform)
        {
            return new Platforms
            {
                platform
            };
        }
    }
}
