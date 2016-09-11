using System;
using System.Collections.Generic;
using System.Linq;
using LocalAppVeyor.Configuration.Model;
using LocalAppVeyor.Configuration.Reader;
using LocalAppVeyor.Pipeline.Internal;

namespace LocalAppVeyor.Pipeline
{
    public sealed class Engine
    {
        public event UnhandledStepExceptionHandler UnhandledStepExceptionReceived;

        private readonly BuildConfiguration buildConfiguration;

        private readonly EngineConfiguration engineConfiguration;

        public Engine(
            EngineConfiguration engineConfiguration,
            IBuildConfigurationReader buildConfigurationReader)
            : this(engineConfiguration, buildConfigurationReader.GetBuildConfiguration())
        {
        }

        public Engine(
            EngineConfiguration engineConfiguration,
            BuildConfiguration buildConfiguration)
        {
            if (engineConfiguration == null) throw new ArgumentNullException(nameof(engineConfiguration));
            if (buildConfiguration == null) throw new ArgumentNullException(nameof(buildConfiguration));

            this.buildConfiguration = buildConfiguration;
            this.engineConfiguration = engineConfiguration;
        }

        public bool Start()
        {
            var executionContext = new ExecutionContext(
                buildConfiguration,
                engineConfiguration.Outputter,
                engineConfiguration.RepositoryDirectoryPath,
                !string.IsNullOrEmpty(buildConfiguration.CloneFolder) ? buildConfiguration.CloneFolder : @"C:\Projects\LocalAppVeyorTempClone");

            var environmentsVariables = buildConfiguration.EnvironmentVariables.Matrix.Count > 0
                ? buildConfiguration.EnvironmentVariables.Matrix.ToArray()
                : new IReadOnlyCollection<Variable>[] { null };
            var configurations = buildConfiguration.Configurations.Count > 0
                ? buildConfiguration.Configurations.ToArray()
                : new string[] { null };
            var platforms = buildConfiguration.Platforms.Count > 0
                ? buildConfiguration.Platforms.ToArray()
                : new string[] { null };
            var oses = buildConfiguration.OperatingSystems.Count > 0
                ? buildConfiguration.OperatingSystems.ToArray()
                : new string[] { null };

            // lets get inside the matrix
            foreach (var environmentVariables in environmentsVariables)
            foreach (var configuration in configurations)
            foreach (var platform in platforms)
            foreach (var os in oses)
            {
                // Update context with new build state
                executionContext.CurrentJob = new MatrixJob(environmentVariables, configuration, platform, os);

                try
                {
                    ExecuteBuild(executionContext);
                }
                catch (Exception e)
                {
                    executionContext.Outputter.WriteError($"Unhandled exception: {e.Message}");

                    var eventArgs = new UnhandledStepExceptionEventArgs(e);
                    UnhandledStepExceptionReceived?.Invoke(this, eventArgs);
                                
                    // someone handled this exception and chose to continue
                    if (eventArgs.ContinueExecution)
                    {
                        executionContext.Outputter.WriteWarning("Continuing executing after recovering from exception...");
                    }
                    // fast_finish is set on configuration
                    else if (buildConfiguration.Matrix.IsFastFinish)
                    {
                        goto FastFinish;
                    }
                    // otherwise just continue to next build from matrix
                    else
                    {
                        break;
                    }
                }
            }

            engineConfiguration.Outputter.Write("Build execution finished.");
            return true;

            FastFinish:
            {
                return false;
            }
        }

        private void ExecuteBuild(ExecutionContext executionContext)
        {
            // initialize standard variables
            ExecuteInternalStep(new InitStandardEnvironmentVariablesStep(), executionContext);

            // initialize environment variables (both common and build specific)
            foreach (
                var variable
                in buildConfiguration.EnvironmentVariables.CommonVariables.Concat(executionContext.CurrentJob.Variables))
            {
                Environment.SetEnvironmentVariable(variable.Name, variable.Value);
            }
            
            // Init
            ExecuteInternalStep(new InitStep(), executionContext);

            // Clone
            ExecuteInternalStep(new CloneFolderStep(), executionContext);

            // Install
            ExecuteInternalStep(new InstallStep(), executionContext);

            // Before build
            ExecuteInternalStep(new BeforeBuildStep(), executionContext);

            // Build
            if (buildConfiguration.Build.IsAutomaticBuildOff)
            {
                ExecuteInternalStep(new BuildScriptStep(), executionContext);
            }
            else
            {
                ExecuteInternalStep(new BuildStep(), executionContext);
            }

            // After Build
            ExecuteInternalStep(new AfterBuildStep(), executionContext);
        }

        private void ExecuteInternalStep(InternalEngineStep step, ExecutionContext executionContext)
        {
            step.Execute(executionContext, UnhandledStepExceptionReceived);
        }
    }
}
