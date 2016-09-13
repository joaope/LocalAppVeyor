namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal interface IInternalEngineStep
    {
        bool Execute(ExecutionContext executionContext);
    }
}
