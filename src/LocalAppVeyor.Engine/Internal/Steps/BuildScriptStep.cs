using LocalAppVeyor.Engine.Configuration;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal sealed class BuildScriptStep : ScriptBlockExecuterStep
    {
        public BuildScriptStep(string workigDirectory, ScriptBlock scriptBlock) 
            : base(workigDirectory, scriptBlock)
        {
        }
    }
}
