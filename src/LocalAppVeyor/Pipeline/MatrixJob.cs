using System.Collections.Generic;
using LocalAppVeyor.Configuration.Model;

namespace LocalAppVeyor.Pipeline
{
    public sealed class MatrixJob
    {
        public string CurrentBuildOperatingSystem { get; }

        public string CurrentBuildPlatform { get; }

        public string CurrentBuildConfiguration { get; }

        public IReadOnlyCollection<Variable> CurrentBuildSpecificVariables { get; }

        public MatrixJob(
            IReadOnlyCollection<Variable> variables,
            string configuration,
            string platform,
            string operatingSystem)
        {
            CurrentBuildOperatingSystem = operatingSystem;
            CurrentBuildPlatform = platform;
            CurrentBuildConfiguration = configuration;
            CurrentBuildSpecificVariables = variables;
        }
    }
}