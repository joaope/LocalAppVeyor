using System;

namespace LocalAppVeyor.Engine.Pipeline
{
    public class UnhandledStepExceptionEventArgs
    {
        public Exception UnhandledException { get; }
        
        public bool ContinueExecution { get; set; }

        public UnhandledStepExceptionEventArgs(Exception unhandledException)
        {
            UnhandledException = unhandledException;
        }
    }
}