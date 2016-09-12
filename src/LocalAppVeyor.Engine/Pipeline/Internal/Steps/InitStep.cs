using LocalAppVeyor.Engine.Configuration.Model;

namespace LocalAppVeyor.Engine.Pipeline.Internal.Steps
{
    internal class InitStep : ScriptBlockExecuterStep
    {
        public InitStep(ScriptBlock scriptBlock)
            : base(scriptBlock)
        {
        }
    }
}
