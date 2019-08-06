using System.IO.Abstractions;
using LocalAppVeyor.Engine.Configuration;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal class InitStep : ScriptBlockExecuterStep
    {
        public InitStep(IFileSystem fileSystem, string workigDirectory, ScriptBlock scriptBlock)
            : base(fileSystem, workigDirectory, scriptBlock)
        {
        }
    }
}
