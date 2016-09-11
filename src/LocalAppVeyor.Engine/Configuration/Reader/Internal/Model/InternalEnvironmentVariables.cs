using System.Collections.ObjectModel;
using System.Linq;
using LocalAppVeyor.Configuration.Model;

namespace LocalAppVeyor.Configuration.Reader.Internal.Model
{
    internal class InternalEnvironmentVariables
    {
        public readonly Collection<InternalVariable> InternalCommonVariables = new Collection<InternalVariable>();

        public readonly Collection<ReadOnlyCollection<InternalVariable>> InternalMatrix =
            new Collection<ReadOnlyCollection<InternalVariable>>();

        public EnvironmentVariables ToEnvironmentVariables()
        {
            return new EnvironmentVariables(
                InternalCommonVariables.Select(v => v.ToVariable()),
                InternalMatrix.Select(m => new ReadOnlyCollection<Variable>(m.Select(v => v.ToVariable()).ToList())));
        }
    }
}