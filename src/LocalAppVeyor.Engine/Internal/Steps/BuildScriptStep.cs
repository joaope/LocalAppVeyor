using System.IO.Abstractions;
using LocalAppVeyor.Engine.Configuration;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal sealed class BuildScriptStep : ScriptBlockExecuterStep
    {
        public BuildScriptStep(IFileSystem fileSystem, string workigDirectory, ScriptBlock scriptBlock) 
            : base(fileSystem, workigDirectory, scriptBlock)
        {
        }
    }
}
