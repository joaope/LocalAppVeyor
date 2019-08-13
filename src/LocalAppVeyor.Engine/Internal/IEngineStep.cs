namespace LocalAppVeyor.Engine.Internal
{
    internal interface IEngineStep
    {
        string Name { get; }

        bool Execute(ExecutionContext executionContext);
    }
}
