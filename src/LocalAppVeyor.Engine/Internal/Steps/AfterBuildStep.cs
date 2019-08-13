using LocalAppVeyor.Engine.Configuration;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal sealed class AfterBuildStep : ScriptBlockExecuterStep
    {
        public AfterBuildStep(string workigDirectory, ScriptBlock scriptBlock) 
            : base(workigDirectory, scriptBlock)
        {
        }
    }
}