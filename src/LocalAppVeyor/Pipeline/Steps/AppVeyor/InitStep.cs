using LocalAppVeyor.Configuration.Model;

namespace LocalAppVeyor.Pipeline.Steps.AppVeyor
{
    public class InitStep : ScriptBlockExecuterStep
    {
        protected override ScriptBlock ScriptBlock { get; }

        protected override bool IncludeEnvironmentVariables => true;

        public InitStep(BuildConfiguration buildConfiguration) 
            : base(buildConfiguration)
        {
            ScriptBlock = buildConfiguration.InitializationScript;
        }
    }
}
