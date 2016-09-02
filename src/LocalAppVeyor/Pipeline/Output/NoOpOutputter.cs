namespace LocalAppVeyor.Pipeline.Output
{
    internal class NoOpOutputter : IPipelineOutputter
    {
        public void Write(string message)
        {
        }

        public void WriteSuccess(string successMessage)
        {
        }

        public void WriteWarning(string warningMessage)
        {
        }

        public void WriteError(string errorMessage)
        {
        }
    }
}
