using System;
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
                    ? _onSuccessStep.Execute(_executionContext)
                    : _onFailureStep.Execute(_executionContext);

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
                _onFinishStep.Execute(_executionContext);
            }

            return executionResult;
        }

        private bool ExecuteBuildPipeline(ExecutionContext executionContext)
        {
            if (!_environmentStep.Execute(executionContext))
            {
                return false;
            }

            if (!_initStep.Execute(executionContext))
            {
                return false;
            }

            if (!_cloneStep.Execute(executionContext))
            {
                return false;
            }

            if (!_installStep.Execute(executionContext))
            {
                return false;
            }

            if (!_assemblyInfoStep.Execute(executionContext))
            {
                return false;
            }

            // Before build
            if (!_beforeBuildStep.Execute(executionContext))
            {
                return false;
            }

            // Build
            if (executionContext.BuildConfiguration.Build.IsAutomaticBuildOff)
            {
                if (!_buildScriptStep.Execute(executionContext))
                {
                    return false;
                }
            }
            else
            {
                if (!_buildStep.Execute(executionContext))
                {
                    return false;
                }
            }

            // After Build
            if (!_afterBuildStep.Execute(executionContext))
            {
                return false;
            }

            // Test script
            if (!_testScriptStep.Execute(executionContext))
            {
                return false;
            }

            return true;
        }
    }
}