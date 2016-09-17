using System;

namespace LocalAppVeyor.Engine
{
    public sealed class JobStartingEventArgs : EventArgs
    {
        public MatrixJob Job { get; }

        public JobStartingEventArgs(MatrixJob job)
        {
            Job = job;
        }  
    }
}