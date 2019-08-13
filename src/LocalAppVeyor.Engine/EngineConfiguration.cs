using System;
using System.IO.Abstractions;

namespace LocalAppVeyor.Engine
{
    public class EngineConfiguration
    {
        public string RepositoryDirectoryPath { get; }

        public string FallbackCloneDirectoryPath { get; }

        public IPipelineOutputter Outputter { get; }

        public IFileSystem FileSystem { get; }

        public EngineConfiguration(
            string repositoryDirectoryPath,
            string fallbackCloneDirectoryPath,
            IPipelineOutputter outputter,
            IFileSystem fileSystem)
        {
            if (string.IsNullOrEmpty(repositoryDirectoryPath)) throw new ArgumentNullException(nameof(repositoryDirectoryPath));
            if (string.IsNullOrEmpty(fallbackCloneDirectoryPath)) throw new ArgumentNullException(nameof(fallbackCloneDirectoryPath));

            RepositoryDirectoryPath = repositoryDirectoryPath;
            FallbackCloneDirectoryPath = fallbackCloneDirectoryPath;
            Outputter = outputter ?? throw new ArgumentNullException(nameof(outputter));
            FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public EngineConfiguration(
            string repositoryDirectoryPath,
            IPipelineOutputter outputter,
            IFileSystem fileSystem)
        : this(repositoryDirectoryPath, GetFallbackTemporaryCloningFolder(fileSystem), outputter, fileSystem)
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

        private static string GetFallbackTemporaryCloningFolder(IFileSystem fileSystem)
        {
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            string tempDirectory;

            do
            {
                tempDirectory = fileSystem.Path.Combine(fileSystem.Path.GetTempPath(), fileSystem.Path.GetRandomFileName());
            } while (fileSystem.Directory.Exists(tempDirectory));

            return tempDirectory;
        }
    }
}
