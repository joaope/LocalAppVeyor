using System;
using LocalAppVeyor.Engine.Configuration.Model;

namespace LocalAppVeyor.Engine.Pipeline.Internal
{
    internal class InstallStep : ScriptBlockExecuterStep
    {
        public override string Name => "Install";

        public override Func<ExecutionContext, ScriptBlock> RetrieveScriptBlock
        {
            get { return context => context.BuildConfiguration.InstallScript; }
        }
    }
}
