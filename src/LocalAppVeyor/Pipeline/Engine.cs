using System;
using System.Linq;
using LocalAppVeyor.Configuration.Model;
using LocalAppVeyor.Configuration.Reader;
using LocalAppVeyor.Pipeline.Internal;
using LocalAppVeyor.Pipeline.Output;

namespace LocalAppVeyor.Pipeline
{
    public sealed class Engine
    {
        public event UnhandledStepExceptionHandler UnhandledStepExceptionReceived;

        private readonly BuildConfiguration buildConfiguration;

        private readonly IPipelineOutputter outputter;

        private readonly EngineConfiguration engineConfiguration;

        public Engine(
            EngineConfiguration engineConfiguration,
            IBuildConfigurationReader buildConfigurationReader,
            IPipelineOutputter outputter)
            : this(engineConfiguration, buildConfigurationReader.GetBuildConfiguration(), outputter)
        {
        }

        public Engine(
            EngineConfiguration engineConfiguration,
            BuildConfiguration buildConfiguration,
            IPipelineOutputter outputter)
        {
            if (engineConfiguration == null) throw new ArgumentNullException(nameof(engineConfiguration));
            if (buildConfiguration == null) throw new ArgumentNullException(nameof(buildConfiguration));
            if (outputter == null) throw new ArgumentNullException(nameof(outputter));

            this.buildConfiguration = buildConfiguration;
            this.engineConfiguration = engineConfiguration;
            this.outputter = outputter;
        }

        public void Start()
        {
            var executionContext = new ExecutionContext(
                buildConfiguration,
                outputter,
                engineConfiguration.RepositoryDirectoryPath);

            // Init
            if (!new InitStep().Execute(executionContext, engineConfiguration.Steps, UnhandledStepExceptionReceived))
            {
                return;
            }

            // Clone
            executionContext.CloneDirectory = !string.IsNullOrEmpty(buildConfiguration.CloneFolder)
                ? buildConfiguration.CloneFolder
                : @"C:\Projects\LocalAppVeyorTempClone";

            if (!new CloneFolderStep().Execute(executionContext, engineConfiguration.Steps, UnhandledStepExceptionReceived))
            {
                return;
            }

            // InitStandardEnvironmentVariables
            if (!new InitStandardEnvironmentVariablesStep().Execute(executionContext, engineConfiguration.Steps, UnhandledStepExceptionReceived))
            {
                return;
            }

            // Install
            if (!new InstallStep().Execute(executionContext, engineConfiguration.Steps, UnhandledStepExceptionReceived))
            {
                return;
            }

            // BeforeBuild
            if (!new BeforeBuildStep().Execute(executionContext, engineConfiguration.Steps, UnhandledStepExceptionReceived))
            {
                return;
            }

            // Build if not off; otherwise run build script
            if (!buildConfiguration.Build.IsAutomaticBuildOff)
            {
                var oses = buildConfiguration.OperatingSystems?.Count > 0
                    ? buildConfiguration.OperatingSystems.ToArray()
                    : new[] { string.Empty };
                var platforms = buildConfiguration.Platforms?.Count > 0
                    ? buildConfiguration.Platforms.ToArray()
                    : new[] { string.Empty };
                var configurations = buildConfiguration.Configurations?.Count > 0
                    ? buildConfiguration.Configurations.ToArray()
                    : new[] { string.Empty };

                foreach (var os in oses)
                {
                    foreach (var platform in platforms)
                    {
                        var buildCounter = 0;

                        foreach (var configuration in configurations)
                        {
                            var variables = buildCounter < buildConfiguration.EnvironmentVariables.Matrix.Count
                                ? buildConfiguration.EnvironmentVariables.Matrix[buildCounter].ToArray()
                                : new Variable[0];

                            executionContext.SetBuildState(true, os, platform, configuration, variables);

                            if (!new BuildStep().Execute(executionContext, engineConfiguration.Steps, UnhandledStepExceptionReceived))
                            {
                                return;
                            }

                            buildCounter++;
                        }
                    }
                }
            }
            else if (!new BuildScriptStep().Execute(executionContext, engineConfiguration.Steps, UnhandledStepExceptionReceived))
            {
                return;
            }

            // AfterBuild
            if (!new AfterBuildStep().Execute(executionContext, engineConfiguration.Steps, UnhandledStepExceptionReceived))
            {
                return;
            }

            outputter.Write("Build execution finished.");
        }
    }
}
