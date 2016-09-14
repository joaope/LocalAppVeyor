using LocalAppVeyor.Engine.Configuration;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal sealed class OnFailureStep : ScriptBlockExecuterStep
    {
        public OnFailureStep(ScriptBlock scriptBlock)
            : base(scriptBlock)
        {
        }
    }
}