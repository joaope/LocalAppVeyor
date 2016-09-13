using LocalAppVeyor.Engine.Configuration;

namespace LocalAppVeyor.Engine.Internal.Steps
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
                    if (string.IsNullOrEmpty(scriptLine.Script))
                    {
                        return true;
                    }

                    switch (scriptLine.ScriptType)
                    {
                        case ScriptType.Batch:
                            return ExecuteBatchScript(executionContext, scriptLine.Script);
                        case ScriptType.PowerShell:
                            return ExecutePowerShellScript(executionContext, scriptLine.Script);
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
