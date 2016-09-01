using LocalAppVeyor.Configuration.Model;

namespace LocalAppVeyor.Pipeline
{
    public abstract class Step
    {
        protected Step(BuildConfiguration buildConfiguration)
        {
        }

        public abstract bool Execute(ExecutionContext executionContext);
    }
}
