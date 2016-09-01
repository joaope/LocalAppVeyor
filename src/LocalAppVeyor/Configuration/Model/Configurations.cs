using System.Collections.Generic;

namespace LocalAppVeyor.Configuration.Model
{
    public class Configurations : List<string>
    {
        public static implicit operator Configurations(string platform)
        {
            return new Configurations
            {
                platform
            };
        }
    }
}