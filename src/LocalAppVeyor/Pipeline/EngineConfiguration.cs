using System;
using System.Collections.Generic;
using System.Linq;

namespace LocalAppVeyor.Pipeline
{
    public class EngineConfiguration
    {
        public IEngineStep[] Steps { get; }

        public string RepositoryDirectoryPath { get; }

        public EngineConfiguration(
            string repositoryDirectoryPath,
            IEnumerable<IEngineStep> steps)
        {
            if (steps == null) throw new ArgumentNullException(nameof(steps));

            RepositoryDirectoryPath = repositoryDirectoryPath;
            Steps = steps.ToArray();
        }
    }
}
