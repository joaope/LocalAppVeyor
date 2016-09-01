using LocalAppVeyor.Configuration.Model;

namespace LocalAppVeyor.Pipeline.Steps.AppVeyor
{
    public class InitStandardEnvironmentVariablesStep : Step
    {
        public InitStandardEnvironmentVariablesStep(BuildConfiguration buildConfiguration) 
            : base(buildConfiguration)
        {
        }

        public override bool Execute(ExecutionContext executionContext)
        {
            executionContext.Outputter.Write("Initializing environment variables...");

            executionContext.UpsertEnvironmentVariable("APPVEYOR_BUILD_FOLDER", executionContext.WorkingDirectory);
            executionContext.UpsertEnvironmentVariable("CI", "False");
            executionContext.UpsertEnvironmentVariable("APPVEYOR", "False");

            executionContext.Outputter.Write("Environment variables initialized.");

            return true;
        }
    }
}
