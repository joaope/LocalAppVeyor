using LocalAppVeyor.Engine.Configuration;
using LocalAppVeyor.Engine.IO;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal sealed class TestScriptStep : ScriptBlockExecuterStep
    {
        public TestScriptStep(FileSystem fileSystem, ScriptBlock scriptBlock) 
            : base(fileSystem, scriptBlock)
        {
        }
    }
}
