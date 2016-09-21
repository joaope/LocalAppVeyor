using LocalAppVeyor.Engine.Configuration;
using LocalAppVeyor.Engine.IO;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal class InstallStep : ScriptBlockExecuterStep
    {
        public InstallStep(FileSystem fileSystem, ScriptBlock scriptBlock) 
            : base(fileSystem, scriptBlock)
        {
        }
    }
}
