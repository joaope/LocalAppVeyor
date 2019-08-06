using System.IO.Abstractions;
using LocalAppVeyor.Engine.Configuration;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal sealed class TestScriptStep : ScriptBlockExecuterStep
    {
        public TestScriptStep(IFileSystem fileSystem, string workigDirectory, ScriptBlock scriptBlock) 
            : base(fileSystem, workigDirectory, scriptBlock)
        {
        }
    }
}
