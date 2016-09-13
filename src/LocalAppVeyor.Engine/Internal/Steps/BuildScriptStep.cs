using LocalAppVeyor.Engine.Configuration;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal sealed class BuildScriptStep : ScriptBlockExecuterStep
    {
        public BuildScriptStep(ScriptBlock scriptBlock) 
            : base(scriptBlock)
        {
        }
    }
}
