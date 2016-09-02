namespace LocalAppVeyor.Pipeline.Internal
{
    internal abstract class InternalEngineStep : IEngineStep
    {
        public abstract string Name { get; }

        public virtual bool ContinueOnFail => false;

        public string BeforeStepName => null;

        public string AfterStepName => null;

        public abstract bool Execute(ExecutionContext executionContext);
    }
}
