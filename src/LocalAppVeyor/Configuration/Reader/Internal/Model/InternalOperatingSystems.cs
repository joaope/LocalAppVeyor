using System.Collections.Generic;

namespace LocalAppVeyor.Configuration.Reader.Internal.Model
{
    internal class InternalOperatingSystems : List<string>
    {
        public static implicit operator InternalOperatingSystems(string os)
        {
            return new InternalOperatingSystems
            {
                os
            };
        }
    }
}
