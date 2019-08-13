using System;
using System.Linq;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal class InitStandardEnvironmentVariablesStep : IEngineStep
    {
        public string Name => "environment";

        public bool Execute(ExecutionContext executionContext)
        {
            executionContext.Outputter.Write("Initializing environment variables...");

            Environment.SetEnvironmentVariable("APPVEYOR_BUILD_NUMBER", "0");
            Environment.SetEnvironmentVariable("APPVEYOR_BUILD_VERSION", executionContext.BuildConfiguration.Version);
            Environment.SetEnvironmentVariable("APPVEYOR_BUILD_FOLDER", executionContext.CloneDirectory);
            Environment.SetEnvironmentVariable("CI", "False");
            Environment.SetEnvironmentVariable("APPVEYOR", "False");

            if (!string.IsNullOrEmpty(executionContext.CurrentJob.Configuration))
            {
                Environment.SetEnvironmentVariable("CONFIGURATION", executionContext.CurrentJob.Configuration);
            }

            if (!string.IsNullOrEmpty(executionContext.CurrentJob.Platform))
            {
                Environment.SetEnvironmentVariable("PLATFORM", executionContext.CurrentJob.Platform);
            }

            foreach (
                var variable
                in executionContext.BuildConfiguration.EnvironmentVariables.CommonVariables.Concat(executionContext.CurrentJob.Variables))
            {
                Environment.SetEnvironmentVariable(variable.Name, variable.Value);
            }

            executionContext.Outputter.Write("Environment variables initialized.");

            return true;
        }
    }
}
