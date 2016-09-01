namespace LocalAppVeyor.Pipeline.Output
{
    public interface IPipelineOutputter
    {
        void Write(string message);

        void WriteError(string errorMessage);
    }
}
