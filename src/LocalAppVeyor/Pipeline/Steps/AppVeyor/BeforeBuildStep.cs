using LocalAppVeyor.Configuration.Model;

namespace LocalAppVeyor.Pipeline.Steps.AppVeyor
{
    public class BeforeBuildStep : ScriptBlockExecuterStep
    {
        protected override ScriptBlock ScriptBlock { get; }

        protected override bool IncludeEnvironmentVariables => true;

        public BeforeBuildStep(BuildConfiguration buildConfiguration) 
            : base(buildConfiguration)
        {
            ScriptBlock = buildConfiguration.BeforeBuildScript;
        }
    }
}
