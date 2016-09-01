using LocalAppVeyor.Configuration.Model;

namespace LocalAppVeyor.Pipeline.Steps.AppVeyor
{
    public class InstallStep : ScriptBlockExecuterStep
    {
        protected override ScriptBlock ScriptBlock { get; }

        protected override bool IncludeEnvironmentVariables => true;

        public InstallStep(BuildConfiguration buildConfiguration) 
            : base(buildConfiguration)
        {
            ScriptBlock = buildConfiguration.InstallScript;
        }
    }
}
