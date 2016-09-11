using System.Collections.ObjectModel;
using System.Linq;
using LocalAppVeyor.Engine.Configuration.Model;

namespace LocalAppVeyor.Engine.Configuration.Reader.Internal.Model
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