using System;
using LocalAppVeyor.Configuration.Model;
using LocalAppVeyor.Pipeline.Output;

namespace LocalAppVeyor.Pipeline
{
    public sealed class ExecutionContext
    {
        public bool IsBuildRunning { get; internal set; }

        public string CurrentBuildOperatingSystem { get; internal set; }

        public string CurrentBuildPlatform { get; internal set; }

        public string CurrentBuildConfiguration { get; internal set; }

        public Variable[] CurrentBuildSpecificVariables { get; set; }
        
        public string RepositoryDirectory { get; }

        public string CloneDirectory { get; internal set; }

        public BuildConfiguration BuildConfiguration { get; }

        public IPipelineOutputter Outputter { get; }

        public ExecutionContext(
            BuildConfiguration buildConfiguration,
            IPipelineOutputter outputter,
            string repositoryDirectory)
        {
            if (buildConfiguration == null) throw new ArgumentNullException(nameof(buildConfiguration));
            if (outputter == null) throw new ArgumentNullException(nameof(outputter));
            if (repositoryDirectory == null) throw new ArgumentNullException(nameof(repositoryDirectory));

            BuildConfiguration = buildConfiguration;
            Outputter = outputter;
            RepositoryDirectory = repositoryDirectory;
        }

        internal void SetBuildState(
            bool isBuilding,
            string operatingSystem = "",
            string platform = "",
            string configuration = "",
            Variable[] variables = null)
        {
            IsBuildRunning = isBuilding;

            CurrentBuildOperatingSystem = operatingSystem;
            CurrentBuildPlatform = platform;
            CurrentBuildConfiguration = configuration;
            CurrentBuildSpecificVariables = variables ?? new Variable[0];
        }
    }
}
