namespace LocalAppVeyor.Pipeline
{
    public interface IEngineStep
    {
        string Name { get; }

        bool ContinueOnFail { get; }

        string BeforeStepName { get; }

        string AfterStepName { get; }

        bool Execute(ExecutionContext executionContext);
    }
}
