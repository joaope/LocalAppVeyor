using LocalAppVeyor.Pipeline.Output;

namespace LocalAppVeyor.Console
{
    public class ConsoleOutputter : IPipelineOutputter
    {
        public void Write(string message)
        {
            System.Console.WriteLine($"{message ?? "<null>"}");
        }

        public void WriteError(string errorMessage)
        {
            System.Console.WriteLine($"error : {errorMessage ?? "<null>"}");
        }
    }
}
