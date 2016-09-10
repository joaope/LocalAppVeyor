using System;
using System.Collections.Generic;
using LocalAppVeyor.Configuration.Model;
using LocalAppVeyor.Pipeline.Output;

namespace LocalAppVeyor.Pipeline
{
    public sealed class ExecutionContext
    {
        public string CurrentBuildOperatingSystem { get; private set; }

        public string CurrentBuildPlatform { get; private set; }

        public string CurrentBuildConfiguration { get; private set; }

        public IReadOnlyCollection<Variable> CurrentBuildSpecificVariables { get; private set; }
        
        public string RepositoryDirectory { get; private set; }

        public string CloneDirectory { get; private set; }

        public BuildConfiguration BuildConfiguration { get; private set; }

        public IPipelineOutputter Outputter { get; private set; }

        public ExecutionContext(
            BuildConfiguration buildConfiguration,
            IPipelineOutputter outputter,
            string repositoryDirectory,
            string cloneDirectory)
        {
            if (buildConfiguration == null) throw new ArgumentNullException(nameof(buildConfiguration));
            if (outputter == null) throw new ArgumentNullException(nameof(outputter));
            if (repositoryDirectory == null) throw new ArgumentNullException(nameof(repositoryDirectory));

            BuildConfiguration = buildConfiguration;
            Outputter = outputter;
            RepositoryDirectory = repositoryDirectory;
            CloneDirectory = cloneDirectory;
        }

        internal void SetBuildState(
            IReadOnlyCollection<Variable> variables,
            string configuration,
            string platform,
            string operatingSystem)
        {
            CurrentBuildOperatingSystem = operatingSystem;
            CurrentBuildPlatform = platform;
            CurrentBuildConfiguration = configuration;
            CurrentBuildSpecificVariables = variables ?? new Variable[0];
        }
    }
}
