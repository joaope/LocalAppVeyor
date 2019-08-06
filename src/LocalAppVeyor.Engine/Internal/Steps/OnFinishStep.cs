using System.IO.Abstractions;
using LocalAppVeyor.Engine.Configuration;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal sealed class OnFinishStep : ScriptBlockExecuterStep
    {
        public OnFinishStep(IFileSystem fileSystem, string workigDirectory, ScriptBlock scriptBlock)
            : base(fileSystem, workigDirectory, scriptBlock)
        {
        }
    }
}