using System;

namespace LocalAppVeyor.Engine
{
    public sealed class JobExecutionResult
    {
        public MatrixJob Job { get; private set; }

        public JobExecutionResultType ResultType { get; private set; }

        public bool IsSuccessfulExecution => ResultType == JobExecutionResultType.Success;

        public Exception UnhandledException { get; private set; }
        
        private JobExecutionResult ()
        {
        }

        internal static JobExecutionResult CreateSuccess(MatrixJob job)
        {
            return new JobExecutionResult
            {
                Job = job,
                ResultType = JobExecutionResultType.Success
            };
        }

        internal static JobExecutionResult CreateFailure(MatrixJob job)
        {
            return new JobExecutionResult
            {
                Job = job,
                ResultType = JobExecutionResultType.Failure
            };
        }

        internal static JobExecutionResult CreateNotExecuted(MatrixJob job)
        {
            return new JobExecutionResult
            {
                Job = job,
                ResultType = JobExecutionResultType.NotExecuted
            };
        }

        internal static JobExecutionResult CreateUnhandledException(MatrixJob job, Exception exception)
        {
            return new JobExecutionResult
            {
                Job = job,
                ResultType = JobExecutionResultType.UnhandledException,
                UnhandledException = exception
            };
        }

        internal static JobExecutionResult CreateSolutionNotFound(MatrixJob job)
        {
            return new JobExecutionResult
            {
                Job = job,
                ResultType = JobExecutionResultType.SolutionFileNotFound
            };
        }

        internal static JobExecutionResult CreateJobNotFound()
        {
            return new JobExecutionResult
            {
                ResultType = JobExecutionResultType.JobNotFound
            };
        }
    }
}
