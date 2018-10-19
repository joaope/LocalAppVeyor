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

        private readonly BuildConfiguration _buildConfiguration;

        private readonly EngineConfiguration _engineConfiguration;

        private MatrixJob[] _jobs;

        public MatrixJob[] Jobs
        {
            get
            {
                if (_jobs != null)
                {
                    return _jobs;
                }

                var environmentsVariables = _buildConfiguration.EnvironmentVariables.Matrix.Count > 0
                    ? _buildConfiguration.EnvironmentVariables.Matrix.ToArray()
                    : new IReadOnlyCollection<Variable>[] { null };
                var configurations = _buildConfiguration.Configurations.Count > 0
                    ? _buildConfiguration.Configurations.ToArray()
                    : new string[] { null };
                var platforms = _buildConfiguration.Platforms.Count > 0
                    ? _buildConfiguration.Platforms.ToArray()
                    : new string[] { null };
                var oses = _buildConfiguration.OperatingSystems.Count > 0
                    ? _buildConfiguration.OperatingSystems.ToArray()
                    : new string[] { null };

                _jobs = (
                        from environmentVariables in environmentsVariables
                        from configuration in configurations
                        from platform in platforms
                        from os in oses
                        select new MatrixJob(os, environmentVariables, configuration, platform))
                    .ToArray();

                return _jobs;
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
            _buildConfiguration = buildConfiguration ?? throw new ArgumentNullException(nameof(buildConfiguration));
            _engineConfiguration = engineConfiguration ?? throw new ArgumentNullException(nameof(engineConfiguration));
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
                _buildConfiguration,
                _engineConfiguration.Outputter,
                _engineConfiguration.RepositoryDirectoryPath,
                !string.IsNullOrEmpty(_buildConfiguration.CloneFolder)
                    ? _buildConfiguration.CloneFolder
                    : new ExpandableString(@"C:\Projects\LocalAppVeyorTempClone"));

            JobExecutionResult executionResult;

            try
            {
                var isSuccess = ExecuteBuildPipeline(executionContext);

                // on_success / on_failure only happen here, after we know the build status
                // they do intervene on build final status though
                isSuccess = isSuccess
                    ? new OnSuccessStep(_engineConfiguration.FileSystem, executionContext.CloneDirectory, _buildConfiguration.OnSuccessScript).Execute(executionContext)
                    : new OnFailureStep(_engineConfiguration.FileSystem, executionContext.CloneDirectory, _buildConfiguration.OnFailureScript).Execute(executionContext);

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
                new OnFinishStep(_engineConfiguration.FileSystem, executionContext.CloneDirectory, _buildConfiguration.OnFinishScript).Execute(executionContext);
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
                    _buildConfiguration.Matrix.AllowedFailures.Any(a => a.AreConditionsMetForJob(job)))
                {
                    continue;
                }

                // if fast_finish is on mark remaining jobs as NotExecuted and leave build
                if (_buildConfiguration.Matrix.IsFastFinish)
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
            if (!new InitStep(_engineConfiguration.FileSystem, executionContext.RepositoryDirectory, _buildConfiguration.InitializationScript).Execute(executionContext))
            {
                return false;
            }

            // Clone
            if (!new CloneFolderStep(_engineConfiguration.FileSystem).Execute(executionContext))
            {
                return false;
            }

            // Install
            if (!new InstallStep(_engineConfiguration.FileSystem, executionContext.CloneDirectory, _buildConfiguration.InstallScript).Execute(executionContext))
            {
                return false;
            }

            // AssemblyInfo rewrite
            if (!new AssemblyInfoRewriteStep(_engineConfiguration.FileSystem).Execute(executionContext))
            {
                return false;
            }

            // Before build
            if (!new BeforeBuildStep(_engineConfiguration.FileSystem, executionContext.CloneDirectory, _buildConfiguration.BeforeBuildScript).Execute(executionContext))
            {
                return false;
            }

            // Build
            if (_buildConfiguration.Build.IsAutomaticBuildOff)
            {
                if (!new BuildScriptStep(_engineConfiguration.FileSystem, executionContext.CloneDirectory, _buildConfiguration.BuildScript).Execute(executionContext))
                {
                    return false;
                }
            }
            else
            {
                if (!new BuildStep(_engineConfiguration.FileSystem).Execute(executionContext))
                {
                    return false;
                }
            }

            // After Build
            if (!new AfterBuildStep(_engineConfiguration.FileSystem, executionContext.CloneDirectory, _buildConfiguration.AfterBuildScript).Execute(executionContext))
            {
                return false;
            }

            // Test script
            if (!new TestScriptStep(_engineConfiguration.FileSystem, executionContext.CloneDirectory, _buildConfiguration.TestScript).Execute(executionContext))
            {
                return false;
            }

            return true;
        }
    }
}
