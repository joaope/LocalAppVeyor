using LocalAppVeyor.Engine.Configuration;
using LocalAppVeyor.Engine.IO;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal sealed class OnFailureStep : ScriptBlockExecuterStep
    {
        public OnFailureStep(FileSystem fileSystem, string workigDirectory, ScriptBlock scriptBlock)
            : base(fileSystem, workigDirectory, scriptBlock)
        {
        }
    }
}