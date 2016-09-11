using System;
using LocalAppVeyor.Engine.Pipeline;

namespace LocalAppVeyor
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
            SetColor(ConsoleColor.Green);
            System.Console.WriteLine($"{successMessage ?? "<null>"}");
            ResetColor();
        }

        public void WriteWarning(string warningMessage)
        {
            SetColor(ConsoleColor.Yellow);
            System.Console.WriteLine($"{warningMessage ?? "<null>"}");
            ResetColor();
        }

        public void WriteError(string errorMessage)
        {
            SetColor(ConsoleColor.Red);
            System.Console.WriteLine($"{errorMessage ?? "<null>"}");
            ResetColor();
        }
    }
}
