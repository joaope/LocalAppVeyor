using System;

namespace LocalAppVeyor.Engine
{
    public class LocalAppVeyorException : Exception
    {
        public LocalAppVeyorException()
        {
        }

        public LocalAppVeyorException(string message)
            : base(message)
        {
        }
        
        public LocalAppVeyorException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}