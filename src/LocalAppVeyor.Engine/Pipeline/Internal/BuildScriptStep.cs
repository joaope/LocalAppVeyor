using System;
using LocalAppVeyor.Engine.Configuration.Model;

namespace LocalAppVeyor.Engine.Pipeline.Internal
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
