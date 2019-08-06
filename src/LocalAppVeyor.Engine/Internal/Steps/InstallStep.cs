using System.IO.Abstractions;
using LocalAppVeyor.Engine.Configuration;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal class InstallStep : ScriptBlockExecuterStep
    {
        public InstallStep(IFileSystem fileSystem, string workigDirectory, ScriptBlock scriptBlock) 
            : base(fileSystem, workigDirectory, scriptBlock)
        {
        }
    }
}
