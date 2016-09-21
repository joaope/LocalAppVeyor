using LocalAppVeyor.Engine.Configuration;
using LocalAppVeyor.Engine.IO;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal sealed class AfterBuildStep : ScriptBlockExecuterStep
    {
        public AfterBuildStep(FileSystem fileSystem, ScriptBlock scriptBlock) 
            : base(fileSystem, scriptBlock)
        {
        }
    }
}