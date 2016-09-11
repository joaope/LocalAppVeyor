using System;
using LocalAppVeyor.Configuration.Model;
using LocalAppVeyor.Pipeline.Output;

namespace LocalAppVeyor.Pipeline
{
    public sealed class ExecutionContext
    {
        public MatrixJob CurrentJob { get; internal set; }
        
        public string RepositoryDirectory { get; }

        public string CloneDirectory { get; }

        public BuildConfiguration BuildConfiguration { get; }

        public IPipelineOutputter Outputter { get; }

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
    }
}
