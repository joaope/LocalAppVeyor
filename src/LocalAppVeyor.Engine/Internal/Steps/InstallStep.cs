using LocalAppVeyor.Engine.Configuration.Model;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal class InstallStep : ScriptBlockExecuterStep
    {
        public InstallStep(ScriptBlock scriptBlock) : base(scriptBlock)
        {
        }
    }
}
