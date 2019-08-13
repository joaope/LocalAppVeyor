namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal interface IInternalEngineStep
    {
        string Name { get; }

        bool Execute(ExecutionContext executionContext);
    }
}
