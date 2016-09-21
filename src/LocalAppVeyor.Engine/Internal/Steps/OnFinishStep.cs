using LocalAppVeyor.Engine.Configuration;
using LocalAppVeyor.Engine.IO;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal sealed class OnFinishStep : ScriptBlockExecuterStep
    {
        public OnFinishStep(FileSystem fileSystem, ScriptBlock scriptBlock)
            : base(fileSystem, scriptBlock)
        {
        }
    }
}