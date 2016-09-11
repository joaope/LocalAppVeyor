using System.Collections.Generic;
using LocalAppVeyor.Engine.Configuration.Model;

namespace LocalAppVeyor.Engine.Pipeline
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