using LocalAppVeyor.Engine.Configuration;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal sealed class OnFinishStep : ScriptBlockExecuterStep
    {
        public override string Name => "on_finish";

        public OnFinishStep(string workigDirectory, ScriptBlock scriptBlock)
            : base(workigDirectory, scriptBlock)
        {
        }
    }
}