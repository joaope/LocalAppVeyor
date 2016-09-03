using System.Collections.Generic;

namespace LocalAppVeyor.Configuration.Model
{
    public class OperatingSystems : List<string>
    {
        public static implicit operator OperatingSystems(string os)
        {
            return new OperatingSystems
            {
                os
            };
        }
    }
}
