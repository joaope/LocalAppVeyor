namespace LocalAppVeyor.Pipeline.Internal
{
    internal class InitStandardEnvironmentVariablesStep : InternalEngineStep
    {
        public override string Name => "InitEnvironmentVariables";

        public override bool Execute(ExecutionContext executionContext)
        {
            executionContext.Outputter.Write("Initializing environment variables...");

            System.Environment.SetEnvironmentVariable("APPVEYOR_BUILD_FOLDER", executionContext.CloneDirectory);
            System.Environment.SetEnvironmentVariable("CI", "False");
            System.Environment.SetEnvironmentVariable("APPVEYOR", "False");

            if (!string.IsNullOrEmpty(executionContext.CurrentJob.Configuration))
            {
                System.Environment.SetEnvironmentVariable("CONFIGURATION", executionContext.CurrentJob.Configuration);
            }

            if (!string.IsNullOrEmpty(executionContext.CurrentJob.Platform))
            {
                System.Environment.SetEnvironmentVariable("PLATFORM", executionContext.CurrentJob.Platform);
            }

            executionContext.Outputter.Write("Environment variables initialized.");

            return true;
        }
    }
}
