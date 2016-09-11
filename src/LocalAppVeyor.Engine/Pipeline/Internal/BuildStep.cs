using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;

namespace LocalAppVeyor.Engine.Pipeline.Internal
{
    internal class BuildStep : InternalEngineStep
    {
        public override string Name => "Build";

        public override bool Execute(ExecutionContext executionContext)
        {
            var platform = executionContext.CurrentJob.Platform;
            var configuration = executionContext.CurrentJob.Configuration;
            string slnProjFile;

            if (File.Exists(executionContext.BuildConfiguration.Build.SolutionFile))
            {
                slnProjFile = executionContext.BuildConfiguration.Build.SolutionFile;
            }
            else if (
                !File.Exists(
                    slnProjFile =
                        Path.Combine(executionContext.CloneDirectory,
                            executionContext.BuildConfiguration.Build.SolutionFile)))
            {
                slnProjFile = "";
            }

            if (string.IsNullOrEmpty(slnProjFile))
            {
                slnProjFile = GetProjectOrSolutionFileRecursively();
            }

            // MSBuild

            var globalProperties = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(platform))
            {
                globalProperties.Add("Platform", platform);
            }

            if (!string.IsNullOrEmpty(configuration))
            {
                globalProperties.Add("Configuration", configuration);
            }

            var buildRequest = new BuildRequestData(slnProjFile, globalProperties, null, new[] {"Build"}, null);
            var buildParameters = new BuildParameters
            {
                Loggers = new ILogger[]
                {
                    new PipelineOutputterMsBuildLogger(
                        executionContext.BuildConfiguration.Build.Verbosity, 
                        executionContext.Outputter)
                },
                
            };

            var buildResult = BuildManager.DefaultBuildManager.Build(buildParameters, buildRequest);

            return buildResult.OverallResult == BuildResultCode.Success;
        }

        private string GetProjectOrSolutionFileRecursively()
        {
            return "";
        }
    }
}
