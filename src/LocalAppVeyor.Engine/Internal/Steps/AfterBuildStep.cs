using LocalAppVeyor.Engine.Configuration;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal sealed class AfterBuildStep : ScriptBlockExecuterStep
    {
        public AfterBuildStep(ScriptBlock scriptBlock) 
            : base(scriptBlock)
        {
        }
    }
}