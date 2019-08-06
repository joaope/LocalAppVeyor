using System.IO.Abstractions;
using LocalAppVeyor.Engine.Configuration;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal sealed class OnFailureStep : ScriptBlockExecuterStep
    {
        public OnFailureStep(IFileSystem fileSystem, string workigDirectory, ScriptBlock scriptBlock)
            : base(fileSystem, workigDirectory, scriptBlock)
        {
        }
    }
}