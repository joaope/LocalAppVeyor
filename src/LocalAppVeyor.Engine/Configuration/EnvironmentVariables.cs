using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LocalAppVeyor.Engine.Configuration
{
    public class EnvironmentVariables
    {
        public ReadOnlyCollection<Variable> CommonVariables { get; }

        public ReadOnlyCollection<IReadOnlyCollection<Variable>> Matrix { get; }

        public EnvironmentVariables()
            : this(new Variable[0], new List<IReadOnlyCollection<Variable>>())
        {
        }

        public EnvironmentVariables(
            IEnumerable<Variable> commonVariables,
            IEnumerable<IReadOnlyCollection<Variable>> matrixVariables)
        {
            CommonVariables = new ReadOnlyCollection<Variable>(commonVariables.ToList());
            Matrix = new ReadOnlyCollection<IReadOnlyCollection<Variable>>(matrixVariables.ToList());
        }
    }
}