using LocalAppVeyor.Engine.Configuration;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal sealed class BeforeBuildStep : ScriptBlockExecuterStep
    {
        public BeforeBuildStep(string workigDirectory, ScriptBlock scriptBlock) 
            : base(workigDirectory, scriptBlock)
        {
        }
    }
}
