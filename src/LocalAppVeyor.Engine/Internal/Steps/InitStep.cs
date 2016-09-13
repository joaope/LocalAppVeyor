using LocalAppVeyor.Engine.Configuration;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal class InitStep : ScriptBlockExecuterStep
    {
        public InitStep(ScriptBlock scriptBlock)
            : base(scriptBlock)
        {
        }
    }
}
