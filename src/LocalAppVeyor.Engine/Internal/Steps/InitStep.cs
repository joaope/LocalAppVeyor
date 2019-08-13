using LocalAppVeyor.Engine.Configuration;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal class InitStep : ScriptBlockExecuterStep
    {
        public override string Name => "init";

        public InitStep(string workingDirectory, ScriptBlock scriptBlock)
            : base(workingDirectory, scriptBlock)
        {
        }
    }
}
