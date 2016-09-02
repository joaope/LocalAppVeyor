using System;
using LocalAppVeyor.Pipeline.Output;

namespace LocalAppVeyor.Console
{
    public class ConsoleOutputter : IPipelineOutputter
    {
        public void Write(string message)
        {
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine($"{message ?? "<null>"}");
        }

        public void WriteSuccess(string successMessage)
        {
            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine($"{successMessage ?? "<null>"}");
        }

        public void WriteWarning(string warningMessage)
        {
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine($"{warningMessage ?? "<null>"}");
        }

        public void WriteError(string errorMessage)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($"{errorMessage ?? "<null>"}");
        }
    }
}
