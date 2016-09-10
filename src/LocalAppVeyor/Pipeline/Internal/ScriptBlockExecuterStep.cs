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

                    if (!string.IsNullOrEmpty(scriptLine.Script))
                    {
                        result = ExecuteBatchScript(executionContext, scriptLine.Script);
                    }
                    else if (!string.IsNullOrEmpty(scriptLine.Script))
                    {
                        // TODO: powershell exec
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
    }
}
