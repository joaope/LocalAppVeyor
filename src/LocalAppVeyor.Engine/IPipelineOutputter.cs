using System;

namespace LocalAppVeyor.Engine
{
    public interface IPipelineOutputter
    {
        void SetColor(ConsoleColor color);

        void ResetColor();

        void Write(string message);

        void WriteSuccess(string successMessage);

        void WriteWarning(string warningMessage);

        void WriteError(string errorMessage);
    }
}
