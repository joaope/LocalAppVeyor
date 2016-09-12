namespace LocalAppVeyor.Engine.Pipeline.Internal.Steps
{
    internal interface IInternalEngineStep
    {
        bool Execute(ExecutionContext executionContext);
    }
}
