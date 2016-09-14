using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LocalAppVeyor.Engine.Configuration
{
    public class EnvironmentVariables
    {
        public ReadOnlyCollection<Variable> CommonVariables { get; }

        public ReadOnlyCollection<ReadOnlyCollection<Variable>> Matrix { get; }

        public EnvironmentVariables()
            : this(new Variable[0], new List<ReadOnlyCollection<Variable>>())
        {
        }

        public EnvironmentVariables(
            IEnumerable<Variable> commonVariables,
            IEnumerable<ReadOnlyCollection<Variable>> matrixVariables)
        {
            CommonVariables = new ReadOnlyCollection<Variable>(commonVariables.ToList());
            Matrix = new ReadOnlyCollection<ReadOnlyCollection<Variable>>(matrixVariables.ToList());
        }
    }
}