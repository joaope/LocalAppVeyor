using System;
using System.IO.Abstractions;
using LocalAppVeyor.Engine.Configuration;

namespace LocalAppVeyor.Engine.Internal
{
    internal class ExecutionContext
    {
        public MatrixJob CurrentJob { get; }
        
        public string RepositoryDirectory { get; }

        public ExpandableString CloneDirectory { get; }

        public IFileSystem FileSystem { get; }

        public BuildConfiguration BuildConfiguration { get; }

        public IPipelineOutputter Outputter { get; }

        public ExecutionContext(
            MatrixJob currentJob,
            BuildConfiguration buildConfiguration,
            IPipelineOutputter outputter,
            string repositoryDirectory,
            ExpandableString cloneDirectory,
            IFileSystem fileSystem)
        {
            CurrentJob = currentJob ?? throw new ArgumentNullException(nameof(currentJob));
            BuildConfiguration = buildConfiguration ?? throw new ArgumentNullException(nameof(buildConfiguration));
            Outputter = outputter ?? throw new ArgumentNullException(nameof(outputter));
            RepositoryDirectory = repositoryDirectory ?? throw new ArgumentNullException(nameof(repositoryDirectory));
            CloneDirectory = cloneDirectory;
            FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }
    }
}
