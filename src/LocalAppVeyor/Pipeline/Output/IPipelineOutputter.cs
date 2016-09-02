namespace LocalAppVeyor.Pipeline.Output
{
    public interface IPipelineOutputter
    {
        void Write(string message);

        void WriteSuccess(string successMessage);

        void WriteWarning(string warningMessage);

        void WriteError(string errorMessage);
    }
}
