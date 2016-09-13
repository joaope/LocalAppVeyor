using LocalAppVeyor.Engine.Configuration.Model;

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
