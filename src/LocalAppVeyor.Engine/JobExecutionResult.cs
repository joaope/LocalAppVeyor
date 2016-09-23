using System;

namespace LocalAppVeyor.Engine
{
    public sealed class JobExecutionResult
    {
        public JobExecutionResultType ResultType { get; private set; }

        public bool IsSuccessfulExecution => ResultType == JobExecutionResultType.Success;

        public Exception UnhandledException { get; private set; }
        
        private JobExecutionResult ()
        {
        }

        internal static JobExecutionResult CreateSuccess()
        {
            return new JobExecutionResult
            {
                ResultType = JobExecutionResultType.Success
            };
        }

        internal static JobExecutionResult CreateFailure()
        {
            return new JobExecutionResult
            {
                ResultType = JobExecutionResultType.Failure
            };
        }

        internal static JobExecutionResult CreateNotExecuted()
        {
            return new JobExecutionResult
            {
                ResultType = JobExecutionResultType.NotExecuted
            };
        }

        internal static JobExecutionResult CreateUnhandledException(Exception exception)
        {
            return new JobExecutionResult
            {
                ResultType = JobExecutionResultType.UnhandledException,
                UnhandledException = exception
            };
        }

        internal static JobExecutionResult CreateSolutionNotFound()
        {
            return new JobExecutionResult
            {
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
