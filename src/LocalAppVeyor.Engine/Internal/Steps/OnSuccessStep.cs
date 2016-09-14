using LocalAppVeyor.Engine.Configuration;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal sealed class OnSuccessStep : ScriptBlockExecuterStep
    {
        public OnSuccessStep(ScriptBlock scriptBlock) 
            : base(scriptBlock)
        {
        }
    }
}
