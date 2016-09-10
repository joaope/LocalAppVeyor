namespace LocalAppVeyor.Pipeline.Internal
{
    internal abstract class InternalEngineStep
    {
        public abstract string Name { get; }

        public abstract bool Execute(ExecutionContext executionContext);

        public void Execute(
            ExecutionContext executionContext, 
            UnhandledStepExceptionHandler unhandledExceptionHandler)
        {
            executionContext.Outputter.Write($"Executing '{Name}'...");
            Execute(executionContext);
            executionContext.Outputter.Write($"'{Name}' successfully executed.");
        }
    }
}
