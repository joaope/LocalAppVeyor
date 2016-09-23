using System;

namespace LocalAppVeyor.Engine
{
    public sealed class JobEndedEventArgs : EventArgs
    {
        public MatrixJob Job { get; }

        public JobExecutionResult ExecutionResult { get; }

        public JobEndedEventArgs(MatrixJob job, JobExecutionResult executionResult)
        {
            Job = job;
            ExecutionResult = executionResult;
        }
    }
}