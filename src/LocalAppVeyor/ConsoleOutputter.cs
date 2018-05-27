using System;
using LocalAppVeyor.Engine;

namespace LocalAppVeyor
{
    internal sealed class ConsoleOutputter : IPipelineOutputter
    {
        public void SetColor(ConsoleColor color)
        {
            Console.ForegroundColor = TransformColor(color, Console.BackgroundColor);
        }

        public void ResetColor()
        {
            Console.ResetColor();
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
            Console.WriteLine($"{message ?? "<null>"}");
        }

        public void WriteSuccess(string successMessage)
        {
            SetColor(ConsoleColor.Green);
            Console.WriteLine($"{successMessage ?? "<null>"}");
            ResetColor();
        }

        public void WriteWarning(string warningMessage)
        {
            SetColor(ConsoleColor.Yellow);
            Console.WriteLine($"{warningMessage ?? "<null>"}");
            ResetColor();
        }

        public void WriteError(string errorMessage)
        {
            SetColor(ConsoleColor.Red);
            Console.WriteLine($"{errorMessage ?? "<null>"}");
            ResetColor();
        }
    }
}
