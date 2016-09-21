using LocalAppVeyor.Engine.Configuration;
using LocalAppVeyor.Engine.IO;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal abstract class ScriptBlockExecuterStep : IInternalEngineStep
    {
        private readonly ScriptBlock scriptBlock;

        private readonly FileSystem fileSystem;

        protected ScriptBlockExecuterStep(FileSystem fileSystem, ScriptBlock scriptBlock)
        {
            this.fileSystem = fileSystem;
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

        private bool ExecuteBatchScript(ExecutionContext executionContext, string script)
        {
            return BatchScriptExecuter.Execute(
                fileSystem,
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
