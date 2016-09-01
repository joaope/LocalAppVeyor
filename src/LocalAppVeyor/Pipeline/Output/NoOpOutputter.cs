namespace LocalAppVeyor.Pipeline.Output
{
    internal class NoOpOutputter : IPipelineOutputter
    {
        public void Write(string message)
        {
        }

        public void WriteError(string errorMessage)
        {
        }
    }
}
