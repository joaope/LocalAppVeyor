using System;
using LocalAppVeyor.Configuration.Model;

namespace LocalAppVeyor.Pipeline.Internal
{
    internal abstract class ScriptBlockExecuterStep : InternalEngineStep
    {
        public abstract Func<ExecutionContext, ScriptBlock> RetrieveScriptBlock { get; }

        public override bool Execute(ExecutionContext executionContext)
        {
            var scriptBlock = RetrieveScriptBlock(executionContext);

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

        private bool ExecuteBatchScript(ExecutionContext executionContext, string script)
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

        private bool ExecutePowerShellScript(ExecutionContext executionContext, string script)
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
