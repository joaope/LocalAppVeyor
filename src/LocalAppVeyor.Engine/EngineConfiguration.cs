using System;
using LocalAppVeyor.Engine.IO;

namespace LocalAppVeyor.Engine
{
    public class EngineConfiguration
    {
        public string RepositoryDirectoryPath { get; }

        public IPipelineOutputter Outputter { get; }

        public FileSystem FileSystem { get; }

        public EngineConfiguration(
            string repositoryDirectoryPath,
            IPipelineOutputter outputter,
            FileSystem fileSystem)
        {
            if (string.IsNullOrEmpty(repositoryDirectoryPath)) throw new ArgumentNullException(nameof(repositoryDirectoryPath));
            if (outputter == null) throw new ArgumentNullException(nameof(outputter));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            RepositoryDirectoryPath = repositoryDirectoryPath;
            Outputter = outputter;
            FileSystem = fileSystem;
        }
    }
}
