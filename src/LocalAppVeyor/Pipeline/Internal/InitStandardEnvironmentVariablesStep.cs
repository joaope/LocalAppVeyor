namespace LocalAppVeyor.Pipeline.Internal
{
    internal class InitStandardEnvironmentVariablesStep : InternalEngineStep
    {
        public override string Name => "InitEnvironmentVariables";

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
