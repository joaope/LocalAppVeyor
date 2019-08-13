using System;
using System.Linq;
using LocalAppVeyor.Engine.Internal.KnownExceptions;
using LocalAppVeyor.Engine.Internal.Steps;

namespace LocalAppVeyor.Engine.Internal
{
    internal sealed class BuildPipelineExecuter
    {
        private readonly ExecutionContext _executionContext;

        private readonly InitStandardEnvironmentVariablesStep _environmentStep;
        private readonly InitStep _initStep;
        private readonly CloneFolderStep _cloneStep;
        private readonly InstallStep _installStep;
        private readonly AssemblyInfoRewriteStep _assemblyInfoStep;
        private readonly BeforeBuildStep _beforeBuildStep;
        private readonly BuildScriptStep _buildScriptStep;
        private readonly BuildStep _buildStep;
        private readonly AfterBuildStep _afterBuildStep;
        private readonly TestScriptStep _testScriptStep;

        private readonly OnSuccessStep _onSuccessStep;
        private readonly OnFailureStep _onFailureStep;
        private readonly OnFinishStep _onFinishStep;

        public BuildPipelineExecuter(ExecutionContext executionContext)
        {
            _executionContext = executionContext;

            _environmentStep = new InitStandardEnvironmentVariablesStep();
            _initStep = new InitStep(executionContext.RepositoryDirectory, executionContext.BuildConfiguration.InitializationScript);
            _cloneStep = new CloneFolderStep(executionContext.FileSystem);
            _installStep = new InstallStep(executionContext.CloneDirectory, executionContext.BuildConfiguration.InstallScript);
            _assemblyInfoStep = new AssemblyInfoRewriteStep();
            _beforeBuildStep = new BeforeBuildStep(executionContext.CloneDirectory, executionContext.BuildConfiguration.BeforeBuildScript);
            _buildScriptStep = new BuildScriptStep(executionContext.CloneDirectory, executionContext.BuildConfiguration.BuildScript);
            _buildStep = new BuildStep();
            _afterBuildStep = new AfterBuildStep(executionContext.CloneDirectory, executionContext.BuildConfiguration.AfterBuildScript);
            _testScriptStep = new TestScriptStep(executionContext.CloneDirectory, executionContext.BuildConfiguration.TestScript);

            _onSuccessStep = new OnSuccessStep(executionContext.CloneDirectory, executionContext.BuildConfiguration.OnSuccessScript);
            _onFailureStep = new OnFailureStep(executionContext.CloneDirectory, executionContext.BuildConfiguration.OnFailureScript);
            _onFinishStep = new OnFinishStep(executionContext.CloneDirectory, executionContext.BuildConfiguration.OnFinishScript);
        }

        public JobExecutionResult Execute()
        {
            JobExecutionResult executionResult;

            try
            {
                var isSuccess = ExecuteBuildPipeline(_executionContext);

                // on_success / on_failure only happens here, after we know the build status
                // they do intervene on build final status though
                isSuccess = isSuccess
                    ? Execute(_onSuccessStep, _executionContext)
                    : Execute(_onFailureStep, _executionContext);

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
                Execute(_onFinishStep, _executionContext);
            }

            return executionResult;
        }

        private bool ExecuteBuildPipeline(ExecutionContext executionContext)
        {
            if (!Execute(_environmentStep, executionContext))
            {
                return false;
            }

            if (!Execute(_initStep, executionContext))
            {
                return false;
            }

            if (!Execute(_cloneStep, executionContext))
            {
                return false;
            }

            if (!Execute(_installStep, executionContext))
            {
                return false;
            }

            if (!Execute(_assemblyInfoStep, executionContext))
            {
                return false;
            }

            // Before build
            if (!Execute(_beforeBuildStep, executionContext))
            {
                return false;
            }

            // Build
            if (executionContext.BuildConfiguration.Build.IsAutomaticBuildOff)
            {
                if (!Execute(_buildScriptStep, executionContext))
                {
                    return false;
                }
            }
            else
            {
                if (!Execute(_buildStep, executionContext))
                {
                    return false;
                }
            }

            // After Build
            if (!Execute(_afterBuildStep, executionContext))
            {
                return false;
            }

            // Test script
            if (!Execute(_testScriptStep, executionContext))
            {
                return false;
            }

            return true;
        }

        private bool Execute(IEngineStep step, ExecutionContext executionContext)
        {
            if (executionContext.BuildConfiguration.SkipSteps.Contains(step.Name, StringComparer.InvariantCultureIgnoreCase))
            {
                executionContext.Outputter.Write($"Skipped '{step.Name}' step.");
                return true;
            }

            return step.Execute(executionContext);
        }
    }
}