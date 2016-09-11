using System.Collections.Generic;
using LocalAppVeyor.Configuration.Model;

namespace LocalAppVeyor.Pipeline
{
    public sealed class MatrixJob
    {
        public string OperatingSystem { get; }

        public string Platform { get; }

        public string Configuration { get; }

        public IReadOnlyCollection<Variable> Variables { get; }

        public MatrixJob(
            IReadOnlyCollection<Variable> variables,
            string configuration,
            string platform,
            string operatingSystem)
        {
            OperatingSystem = operatingSystem;
            Platform = platform;
            Configuration = configuration;
            Variables = variables ?? new Variable[0];
        }
    }
}