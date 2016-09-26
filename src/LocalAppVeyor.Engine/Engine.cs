using System;
using System.Collections.Generic;
using System.Linq;
using LocalAppVeyor.Engine.Configuration;
using LocalAppVeyor.Engine.Configuration.Reader;
using LocalAppVeyor.Engine.Internal;
using LocalAppVeyor.Engine.Internal.KnownExceptions;
using LocalAppVeyor.Engine.Internal.Steps;

namespace LocalAppVeyor.Engine
{
    public sealed class Engine
    {
        public event EventHandler<JobStartingEventArgs> JobStarting = delegate { };

        public event EventHandler<JobEndedEventArgs> JobEnded = delegate { };

        private readonly BuildConfiguration buildConfiguration;

        private readonly EngineConfiguration engineConfiguration;

        private MatrixJob[] jobs;

        public MatrixJob[] Jobs
        {
            get
            {
                if (jobs != null)
                {
                    return jobs;
                }

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

                jobs = (
                        from environmentVariables in environmentsVariables
                        from configuration in configurations
                        from platform in platforms
                        from os in oses
                        select new MatrixJob(os, environmentVariables, configuration, platform))
                    .ToArray();

                return jobs;
            }
        }

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

        public JobExecutionResult ExecuteJob(int jobIndex)
        {
            if (jobIndex < 0 || jobIndex >= Jobs.Length)
            {
                var result = JobExecutionResult.CreateJobNotFound();
                JobEnded?.Invoke(this, new JobEndedEventArgs(null, result));
                return result;
            }

            return ExecuteJob(Jobs[jobIndex]);
        }

        public JobExecutionResult ExecuteJob(MatrixJob job)
        {
            JobStarting?.Invoke(this, new JobStartingEventArgs(job));

            var executionContext = new ExecutionContext(
                job,
                buildConfiguration,
                engineConfiguration.Outputter,
                engineConfiguration.RepositoryDirectoryPath,
                !string.IsNullOrEmpty(buildConfiguration.CloneFolder)
                    ? buildConfiguration.CloneFolder
                    : (ExpandableString) @"C:\Projects\LocalAppVeyorTempClone");

            JobExecutionResult executionResult;

            try
            {
                var isSuccess = ExecuteBuildPipeline(executionContext);

                // on_success / on_failure only happen here, after we know the build status
                // they do intervene on build final status though
                isSuccess = isSuccess
                    ? new OnSuccessStep(engineConfiguration.FileSystem, executionContext.CloneDirectory, buildConfiguration.OnSuccessScript).Execute(executionContext)
                    : new OnFailureStep(engineConfiguration.FileSystem, executionContext.CloneDirectory, buildConfiguration.OnFailureScript).Execute(executionContext);

                return isSuccess
                    ? JobExecutionResult.CreateSuccess()
                    : JobExecutionResult.CreateFailure();
            }
            catch (SolutionNotFoundException)
            {
                executionResult = JobExecutionResult.CreateSolutionNotFound();
            }
            catch (Exception e)
            {
                executionResult = JobExecutionResult.CreateUnhandledException(e);
            }
            finally
            {
                // on_finish don't influence build final status so we just run it
                new OnFinishStep(engineConfiguration.FileSystem, executionContext.CloneDirectory, buildConfiguration.OnFinishScript).Execute(executionContext);
            }

            JobEnded?.Invoke(this, new JobEndedEventArgs(job, executionResult));

            return executionResult;
        }

        public JobExecutionResult[] ExecuteAllJobs()
        {
            var results = new JobExecutionResult[Jobs.Length];

            for (var i = 0; i < Jobs.Length; i++)
            {
                var job = Jobs[i];

                results[i] = ExecuteJob(job);

                // if success, or job is on the allowed failures matrix, continue on to next one 
                if (results[i].IsSuccessfulExecution ||
                    buildConfiguration.Matrix.AllowedFailures.Any(a => a.AreConditionsMetForJob(job)))
                {
                    continue;
                }

                // if fast_finish is on mark remaining jobs as NotExecuted and leave build
                if (buildConfiguration.Matrix.IsFastFinish)
                {
                    for (++i; i < Jobs.Length; i++)
                    {
                        results[i] = JobExecutionResult.CreateNotExecuted();
                    }

                    break;
                }
            }

            return results;
        }

        private bool ExecuteBuildPipeline(ExecutionContext executionContext)
        {
            // initialize standard variables
            if (!new InitStandardEnvironmentVariablesStep().Execute(executionContext))
            {
                return false;
            }
            
            // Init
            if (!new InitStep(engineConfiguration.FileSystem, executionContext.RepositoryDirectory, buildConfiguration.InitializationScript).Execute(executionContext))
            {
                return false;
            }

            // Clone
            if (!new CloneFolderStep(engineConfiguration.FileSystem).Execute(executionContext))
            {
                return false;
            }

            // Install
            if (!new InstallStep(engineConfiguration.FileSystem, executionContext.CloneDirectory, buildConfiguration.InstallScript).Execute(executionContext))
            {
                return false;
            }

            // AssemblyInfo rewrite
            if (!new AssemblyInfoRewriteStep(engineConfiguration.FileSystem).Execute(executionContext))
            {
                return false;
            }

            // Before build
            if (!new BeforeBuildStep(engineConfiguration.FileSystem, executionContext.CloneDirectory, buildConfiguration.BeforeBuildScript).Execute(executionContext))
            {
                return false;
            }

            // Build
            if (buildConfiguration.Build.IsAutomaticBuildOff)
            {
                if (!new BuildScriptStep(engineConfiguration.FileSystem, executionContext.CloneDirectory, buildConfiguration.BuildScript).Execute(executionContext))
                {
                    return false;
                }
            }
            else
            {
                if (!new BuildStep(engineConfiguration.FileSystem).Execute(executionContext))
                {
                    return false;
                }
            }

            // After Build
            if (!new AfterBuildStep(engineConfiguration.FileSystem, executionContext.CloneDirectory, buildConfiguration.AfterBuildScript).Execute(executionContext))
            {
                return false;
            }

            // Test script
            if (!new TestScriptStep(engineConfiguration.FileSystem, executionContext.CloneDirectory, buildConfiguration.TestScript).Execute(executionContext))
            {
                return false;
            }

            return true;
        }
    }
}
