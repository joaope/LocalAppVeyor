using LocalAppVeyor.Engine.Configuration.Model;

namespace LocalAppVeyor.Engine.Pipeline.Internal.Steps
{
    internal abstract class ScriptBlockExecuterStep : IInternalEngineStep
    {
        private readonly ScriptBlock scriptBlock;

        protected ScriptBlockExecuterStep(ScriptBlock scriptBlock)
        {
            this.scriptBlock = scriptBlock;
        }

        public bool Execute(ExecutionContext executionContext)
        {
            if (scriptBlock != null)
            {
                foreach (var scriptLine in scriptBlock)
                {
                    var result = true;

                    if (scriptLine.ScriptType == ScriptType.Batch)
                    {
                        result = ExecuteBatchScript(executionContext, scriptLine.Script);
                    }
                    else if (scriptLine.ScriptType == ScriptType.PowerShell)
                    {
                        result = ExecutePowerShellScript(executionContext, scriptLine.Script);
                    }

                    if (!result)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool ExecuteBatchScript(ExecutionContext executionContext, string script)
        {
            return BatchScriptExecuter.Execute(
                executionContext.CloneDirectory,
                script,
                data =>
                {
                    if (data != null)
                    {
                        executionContext.Outputter.Write(data);
                    }
                },
                data =>
                {
                    if (data != null)
                    {
                        executionContext.Outputter.WriteError(data);
                    }
                });
        }

        private static bool ExecutePowerShellScript(ExecutionContext executionContext, string script)
        {
            return PowerShellScriptExecuter.Execute(
                executionContext.CloneDirectory,
                script,
                data =>
                {
                    if (data != null)
                    {
                        executionContext.Outputter.Write(data);
                    }
                },
                data =>
                {
                    if (data != null)
                    {
                        executionContext.Outputter.WriteError(data);
                    }
                });
        }
    }
}
