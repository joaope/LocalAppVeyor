using System;
using LocalAppVeyor.Configuration.Model;
using LocalAppVeyor.Configuration.Readers;
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
            var executionContext = new ExecutionContext(outputter);

            foreach (var buildStep in engineConfiguration.BuildSteps)
            {
                var step = (Step)Activator.CreateInstance(buildStep.StepType, buildConfiguration);

                outputter.Write($"Starting '{buildStep.Name}' step...");

                bool isToContinue;

                try
                {
                    isToContinue = step.Execute(executionContext);
                }
                catch (Exception e)
                {
                    outputter.WriteError($"Unhandled exception: {e.Message}");

                    var eventArgs = new UnhandledStepExceptionEventArgs(e);
                    UnhandledStepExceptionReceived?.Invoke(this, eventArgs);

                    isToContinue = eventArgs.ContinueExecution;
                }

                if (!isToContinue && !buildStep.ContinueOnFail)
                {
                    outputter.WriteError($"Stopping execution after failing '{buildStep.Name}' step.");
                    break;
                }

                outputter.Write($"Finished '{buildStep.Name}' step.");
            }

            outputter.Write("Build execution finished.");
        }
    }
}
