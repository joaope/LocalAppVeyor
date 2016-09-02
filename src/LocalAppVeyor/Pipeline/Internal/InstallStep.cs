using System;
using LocalAppVeyor.Configuration.Model;

namespace LocalAppVeyor.Pipeline.Internal
{
    internal class InstallStep : ScriptBlockExecuterStep
    {
        public override string Name => "Install";

        protected override bool IncludeEnvironmentVariables => true;

        public override Func<ExecutionContext, ScriptBlock> RetrieveScriptBlock
        {
            get { return context => context.BuildConfiguration.InstallScript; }
        }
    }
}
