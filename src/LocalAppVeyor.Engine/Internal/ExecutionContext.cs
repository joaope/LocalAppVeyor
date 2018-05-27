using System;
using LocalAppVeyor.Engine.Configuration;

namespace LocalAppVeyor.Engine.Internal
{
    internal class ExecutionContext
    {
        public MatrixJob CurrentJob { get; }
        
        public string RepositoryDirectory { get; }

        public ExpandableString CloneDirectory { get; }

        public BuildConfiguration BuildConfiguration { get; }

        public IPipelineOutputter Outputter { get; }

        public ExecutionContext(
            MatrixJob currentJob,
            BuildConfiguration buildConfiguration,
            IPipelineOutputter outputter,
            string repositoryDirectory,
            ExpandableString cloneDirectory)
        {
            CurrentJob = currentJob ?? throw new ArgumentNullException(nameof(currentJob));
            BuildConfiguration = buildConfiguration ?? throw new ArgumentNullException(nameof(buildConfiguration));
            Outputter = outputter ?? throw new ArgumentNullException(nameof(outputter));
            RepositoryDirectory = repositoryDirectory ?? throw new ArgumentNullException(nameof(repositoryDirectory));
            CloneDirectory = cloneDirectory;
        }
    }
}
