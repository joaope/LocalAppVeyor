using System;
using LocalAppVeyor.Configuration.Model;

namespace LocalAppVeyor.Pipeline.Internal
{
    internal sealed class BuildScriptStep : ScriptBlockExecuterStep
    {
        public override string Name => "BuildScript";

        public override Func<ExecutionContext, ScriptBlock> RetrieveScriptBlock
        {
            get { return context => context.BuildConfiguration.BuildScript; }
        }
    }
}
