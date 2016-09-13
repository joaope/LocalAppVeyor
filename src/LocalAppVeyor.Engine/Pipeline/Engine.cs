using System;
using System.Collections.Generic;
using System.Linq;
using LocalAppVeyor.Engine.Configuration.Model;
using LocalAppVeyor.Engine.Configuration.Reader;
using LocalAppVeyor.Engine.Pipeline.Internal;
using LocalAppVeyor.Engine.Pipeline.Internal.KnownExceptions;
using LocalAppVeyor.Engine.Pipeline.Internal.Steps;

namespace LocalAppVeyor.Engine.Pipeline
{
    public sealed class JobStartingEventArgs : EventArgs
    {
        public MatrixJob Job { get; }

        public JobStartingEventArgs(MatrixJob job)
        {
            Job = job;
        }  
    }

    public sealed class JobEndedEventArgs : EventArgs
    {
        public MatrixJob Job { get; }

        public JobExecutionResult ExecutionResult { get; set; }

        public JobEndedEventArgs(MatrixJob job, JobExecutionResult executionResult)
        {
            Job = job;
            ExecutionResult = executionResult;
        }
    }

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
                !string.IsNullOrEmpty(buildConfiguration.CloneFolder) ? buildConfiguration.CloneFolder : @"C:\Projects\LocalAppVeyorTempClone");

            JobExecutionResult executionResult;

            try
            {
                executionResult = ExecuteBuildPipeline(executionContext)
                    ? JobExecutionResult.CreateSuccess(job)
                    : JobExecutionResult.CreateFailure(job);
            }
            catch (SolutionNotFoundException)
            {
                executionResult = JobExecutionResult.CreateSolutionNotFound(job);
            }
            catch (Exception e)
            {
                executionResult = JobExecutionResult.CreateUnhandledException(job, e);
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

                var result = results[i] = ExecuteJob(job);

                // if success, continue to next one 
                if (result.ResultType == JobExecutionResultType.Success)
                {
                    continue;
                }

                // if solution was not found it's not worth the trouble of continuing on
                // as the same will happen for remaining jobs, just return same result on all of them
                if (result.ResultType == JobExecutionResultType.SolutionFileNotFound)
                {
                    return Jobs
                        .Select(JobExecutionResult.CreateSolutionNotFound)
                        .ToArray();
                }

                // Something happened. If fast_finish is disabled we continue
                if (!buildConfiguration.Matrix.IsFastFinish)
                {
                    continue;
                }

                // otherwise, mark remaining jobs as NotExecuted and leave build
                for (++i; i < Jobs.Length; i++)
                {
                    results[i] = JobExecutionResult.CreateNotExecuted(Jobs[i]);
                }

                break;
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

            // initialize environment variables (both common and build specific)
            foreach (
                var variable
                in buildConfiguration.EnvironmentVariables.CommonVariables.Concat(executionContext.CurrentJob.Variables))
            {
                Environment.SetEnvironmentVariable(variable.Name, variable.Value);
            }
            
            // Init
            if (!new InitStep(buildConfiguration.InitializationScript).Execute(executionContext))
            {
                return false;
            }

            // Clone
            if (!new CloneFolderStep().Execute(executionContext))
            {
                return false;
            }

            // Install
            if (!new InstallStep(buildConfiguration.InstallScript).Execute(executionContext))
            {
                return false;
            }

            // Before build
            if (new BeforeBuildStep(buildConfiguration.BeforeBuildScript).Execute(executionContext))
            {
                return false;
            }

            // Build
            if (buildConfiguration.Build.IsAutomaticBuildOff)
            {
                if (!new BuildScriptStep(buildConfiguration.BuildScript).Execute(executionContext))
                {
                    return false;
                }
            }
            else
            {
                if (!new BuildStep().Execute(executionContext))
                {
                    return false;
                }
            }

            // After Build
            if (!new AfterBuildStep(buildConfiguration.AfterBuildScript).Execute(executionContext))
            {
                return false;
            }

            return true;
        }
    }
}
