using System;
using LocalAppVeyor.Engine.Configuration.Model;

namespace LocalAppVeyor.Engine.Pipeline.Internal
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