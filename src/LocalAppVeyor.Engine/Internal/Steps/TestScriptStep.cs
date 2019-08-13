using LocalAppVeyor.Engine.Configuration;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal sealed class TestScriptStep : ScriptBlockExecuterStep
    {
        public TestScriptStep(string workigDirectory, ScriptBlock scriptBlock) 
            : base(workigDirectory, scriptBlock)
        {
        }
    }
}
