using System;
using System.IO.Abstractions;

namespace LocalAppVeyor.Engine
{
    public class EngineConfiguration
    {
        public string RepositoryDirectoryPath { get; }

        public IPipelineOutputter Outputter { get; }

        public IFileSystem FileSystem { get; }

        public EngineConfiguration(
            string repositoryDirectoryPath,
            IPipelineOutputter outputter,
            IFileSystem fileSystem)
        {
            if (string.IsNullOrEmpty(repositoryDirectoryPath)) throw new ArgumentNullException(nameof(repositoryDirectoryPath));

            RepositoryDirectoryPath = repositoryDirectoryPath;
            Outputter = outputter ?? throw new ArgumentNullException(nameof(outputter));
            FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public EngineConfiguration(
            string repositoryDirectoryPath,
            IPipelineOutputter outputter)
        : this(repositoryDirectoryPath, outputter, new FileSystem())
        {
        }
    }
}
