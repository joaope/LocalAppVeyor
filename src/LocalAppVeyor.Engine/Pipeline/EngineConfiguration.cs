using System;

namespace LocalAppVeyor.Engine.Pipeline
{
    public class EngineConfiguration
    {
        public string RepositoryDirectoryPath { get; }

        public IPipelineOutputter Outputter { get; }

        public EngineConfiguration(
            string repositoryDirectoryPath,
            IPipelineOutputter outputter)
        {
            if (repositoryDirectoryPath == null) throw new ArgumentNullException(nameof(repositoryDirectoryPath));
            if (outputter == null) throw new ArgumentNullException(nameof(outputter));

            RepositoryDirectoryPath = repositoryDirectoryPath;
            Outputter = outputter;
        }
    }
}
