using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LocalAppVeyor.Engine.Internal.KnownExceptions;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal class BuildStep : IInternalEngineStep
    {
        public bool Execute(ExecutionContext executionContext)
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
                slnProjFile = GetProjectOrSolutionFileRecursively(executionContext);

                if (string.IsNullOrEmpty(slnProjFile))
                {
                    throw new SolutionNotFoundException();
                }
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
                }
            };

            var buildResult = BuildManager.DefaultBuildManager.Build(buildParameters, buildRequest);

            return buildResult.OverallResult == BuildResultCode.Success;
        }

        private string GetProjectOrSolutionFileRecursively(ExecutionContext executionContext)
        {
            // first tries .sln file
            var possibleHit = Directory
                .EnumerateFiles(executionContext.CloneDirectory, "*.sln")
                .FirstOrDefault(f => f.EndsWith("*.sln", StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(possibleHit))
            {
                return possibleHit;
            }

            // finally tries .csproj files
            return Directory
                .EnumerateFiles(executionContext.CloneDirectory, "*.csproj")
                .FirstOrDefault(f => f.EndsWith("*.csproj", StringComparison.OrdinalIgnoreCase));
        }
    }
}
