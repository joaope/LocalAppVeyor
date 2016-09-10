using System;
using LocalAppVeyor.Pipeline.Output;

namespace LocalAppVeyor.Console
{
    public class ConsoleOutputter : IPipelineOutputter
    {
        public void SetColor(ConsoleColor color)
        {
            System.Console.ForegroundColor = TransformColor(color, System.Console.BackgroundColor);
        }

        public void ResetColor()
        {
            System.Console.ResetColor();
        }

        private static ConsoleColor TransformColor(
            ConsoleColor foreground,
            ConsoleColor background)
        {
            if (foreground != background)
            {
                return foreground;
            }

            return background != ConsoleColor.Black
                ? ConsoleColor.Black
                : ConsoleColor.Gray;
        }

        public void Write(string message)
        {
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
