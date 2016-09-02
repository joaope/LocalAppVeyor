using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using LocalAppVeyor.Configuration.Model;
using LocalAppVeyor.Pipeline.Output;

namespace LocalAppVeyor.Pipeline
{
    public sealed class ExecutionContext
    {
        public IPipelineOutputter Outputter { get; }

        public BuildConfiguration BuildConfiguration { get; }

        private readonly Dictionary<string, string> environmentVariables = new Dictionary<string, string>();

        public ReadOnlyDictionary<string, string> EnvironmentVariables { get; }

        public string WorkingDirectory => Directory.GetCurrentDirectory();

        public ExecutionContext(
            IPipelineOutputter outputter,
            BuildConfiguration buildConfiguration)
        {
            if (outputter == null) throw new ArgumentNullException(nameof(outputter));
            if (buildConfiguration == null) throw new ArgumentNullException(nameof(buildConfiguration));

            Outputter = outputter;
            BuildConfiguration = buildConfiguration;
            EnvironmentVariables = new ReadOnlyDictionary<string, string>(environmentVariables);
        }

        public void UpsertEnvironmentVariable(string name, string value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            environmentVariables.Add(name, value);
        }
    }
}
