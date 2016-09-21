using LocalAppVeyor.Engine.Configuration;
using LocalAppVeyor.Engine.IO;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal sealed class BeforeBuildStep : ScriptBlockExecuterStep
    {
        public BeforeBuildStep(FileSystem fileSystem, ScriptBlock scriptBlock) 
            : base(fileSystem, scriptBlock)
        {
        }
    }
}
