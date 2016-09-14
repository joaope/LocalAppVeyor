using LocalAppVeyor.Engine.Configuration;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal sealed class OnFinishStep : ScriptBlockExecuterStep
    {
        public OnFinishStep(ScriptBlock scriptBlock)
            : base(scriptBlock)
        {
        }
    }
}