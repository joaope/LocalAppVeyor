using LocalAppVeyor.Engine.Configuration;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal sealed class OnFailureStep : ScriptBlockExecuterStep
    {
        public override string Name => "on_failure";

        public OnFailureStep(string workigDirectory, ScriptBlock scriptBlock)
            : base(workigDirectory, scriptBlock)
        {
        }
    }
}