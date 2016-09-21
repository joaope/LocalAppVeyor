using LocalAppVeyor.Engine.Configuration;
using LocalAppVeyor.Engine.IO;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal sealed class BuildScriptStep : ScriptBlockExecuterStep
    {
        public BuildScriptStep(FileSystem fileSystem, ScriptBlock scriptBlock) 
            : base(fileSystem, scriptBlock)
        {
        }
    }
}
