using LocalAppVeyor.Engine.Configuration;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal sealed class BeforeBuildStep : ScriptBlockExecuterStep
    {
        public BeforeBuildStep(ScriptBlock scriptBlock) 
            : base(scriptBlock)
        {
        }
    }
}
