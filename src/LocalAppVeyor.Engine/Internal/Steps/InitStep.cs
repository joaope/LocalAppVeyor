using LocalAppVeyor.Engine.Configuration;
using LocalAppVeyor.Engine.IO;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal class InitStep : ScriptBlockExecuterStep
    {
        public InitStep(FileSystem fileSystem, ScriptBlock scriptBlock)
            : base(fileSystem, scriptBlock)
        {
        }
    }
}
