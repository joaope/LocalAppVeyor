using System;

namespace LocalAppVeyor.Pipeline
{
    public class UnhandledStepExceptionEventArgs
    {
        public Exception UnhandledException { get; }

        public string StepName { get; }

        public bool ContinueExecution { get; set; }

        public UnhandledStepExceptionEventArgs(string stepName, Exception unhandledException)
        {
            StepName = stepName;
            UnhandledException = unhandledException;
        }
    }
}