using System.Collections.ObjectModel;

namespace LocalAppVeyor.Configuration.Model
{
    public class EnvironmentVariables
    {
        public ReadOnlyCollection<Variable> CommonVariables { get; }

        internal readonly Collection<Variable> InternalCommonVariables = new Collection<Variable>();

        public ReadOnlyCollection<ReadOnlyCollection<Variable>> Matrix { get; internal set; }

        internal readonly Collection<ReadOnlyCollection<Variable>> InternalMatrix =
            new Collection<ReadOnlyCollection<Variable>>();

        public EnvironmentVariables()
        {
            CommonVariables = new ReadOnlyCollection<Variable>(InternalCommonVariables);
            Matrix = new ReadOnlyCollection<ReadOnlyCollection<Variable>>(InternalMatrix);
        }
    }
}