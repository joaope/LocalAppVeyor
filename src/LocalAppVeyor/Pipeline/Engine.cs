using System;
using System.Linq;
using LocalAppVeyor.Configuration.Model;
using LocalAppVeyor.Configuration.Readers;
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
            var executionContext = new ExecutionContext(outputter, buildConfiguration);

            // Init
            if (!ExecuteInternalStep(new InitStep(), executionContext))
            {
                return;
            }

            // Clone
            if (!ExecuteInternalStep(new CloneFolderStep(), executionContext))
            {
                return;
            }

            // InitStandardEnvironmentVariables
            if (!ExecuteInternalStep(new InitStandardEnvironmentVariablesStep(), executionContext))
            {
                return;
            }

            // Install
            if (!ExecuteInternalStep(new InstallStep(), executionContext))
            {
                return;
            }

            // BeforeBuild
            if (!ExecuteInternalStep(new BeforeBuildStep(), executionContext))
            {
                return;
            }



            outputter.Write("Build execution finished.");
        }

        private bool ExecuteInternalStep(IEngineStep step, ExecutionContext executionContext)
        {
            foreach (var beforeStep in engineConfiguration.Steps.Where(s => s.BeforeStepName == step.Name))
            {
                if (!ExecuteSingleStepLogic(beforeStep, executionContext))
                {
                    return false;
                }
            }

            if (!ExecuteSingleStepLogic(step, executionContext))
            {
                return false;
            }

            foreach (var beforeStep in engineConfiguration.Steps.Where(s => s.AfterStepName == step.Name))
            {
                if (!ExecuteSingleStepLogic(beforeStep, executionContext))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ExecuteSingleStepLogic(IEngineStep step, ExecutionContext executionContext)
        {
            outputter.Write($"Executing '{step.Name}'...");

            try
            {
                if (step.Execute(executionContext))
                {
                    outputter.Write($"'{step.Name}' successfully executed.");
                    return true;
                }

                if (step.ContinueOnFail)
                {
                    outputter.WriteWarning($"'{step.Name}' was not succedeed but execution is continuing anyway...");
                    return true;
                }

                outputter.WriteError($"'{step.Name}' failed. Stopping build...");
                return false;
            }
            catch (Exception e)
            {
                outputter.WriteError($"Unhandled exception on '{step.Name}' step: {e.Message}");

                var eventArgs = new UnhandledStepExceptionEventArgs(step.Name, e);
                UnhandledStepExceptionReceived?.Invoke(this, eventArgs);

                if (eventArgs.ContinueExecution)
                {
                    outputter.WriteWarning($"Continuing executing after recovering from exception...");
                    return true;
                }

                return false;
            }
        }
    }
}
