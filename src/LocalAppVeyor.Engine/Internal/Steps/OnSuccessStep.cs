using System.IO.Abstractions;
using LocalAppVeyor.Engine.Configuration;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal sealed class OnSuccessStep : ScriptBlockExecuterStep
    {
        public OnSuccessStep(IFileSystem fileSystem, string workigDirectory, ScriptBlock scriptBlock) 
            : base(fileSystem, workigDirectory, scriptBlock)
        {
        }
    }
}
