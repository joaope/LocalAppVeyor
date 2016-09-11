using System;
using LocalAppVeyor.Engine.Configuration.Model;

namespace LocalAppVeyor.Engine.Pipeline.Internal
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
