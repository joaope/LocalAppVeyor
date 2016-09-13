using LocalAppVeyor.Engine.Configuration.Model;

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
