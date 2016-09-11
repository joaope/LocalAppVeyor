using System;
using LocalAppVeyor.Engine.Configuration.Model;

namespace LocalAppVeyor.Engine.Pipeline.Internal
{
    internal sealed class BeforeBuildStep : ScriptBlockExecuterStep
    {
        public override string Name => "BeforeBuild";

        public override Func<ExecutionContext, ScriptBlock> RetrieveScriptBlock
        {
            get { return context => context.BuildConfiguration.BeforeBuildScript; }
        }
    }
}
