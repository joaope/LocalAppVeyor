using System;
using LocalAppVeyor.Configuration.Model;

namespace LocalAppVeyor.Pipeline.Internal
{
    internal sealed class AfterBuildStep : ScriptBlockExecuterStep
    {
        public override string Name => "AfterBuild";

        public override Func<ExecutionContext, ScriptBlock> RetrieveScriptBlock
        {
            get { return context => context.BuildConfiguration.AfterBuildScript; }
        }
    }
}