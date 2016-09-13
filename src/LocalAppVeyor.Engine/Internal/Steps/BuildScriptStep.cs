using LocalAppVeyor.Engine.Configuration.Model;

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
