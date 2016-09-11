using System;
using LocalAppVeyor.Configuration.Model;

namespace LocalAppVeyor.Pipeline.Internal
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
