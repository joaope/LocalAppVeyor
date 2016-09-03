using System;
using LocalAppVeyor.Configuration.Model;

namespace LocalAppVeyor.Pipeline.Internal
{
    internal class InitStep : ScriptBlockExecuterStep
    {
        public override string Name => "Init";

        public override Func<ExecutionContext, ScriptBlock> RetrieveScriptBlock
        {
            get { return context => context.BuildConfiguration.InitializationScript; }
        }
    }
}
