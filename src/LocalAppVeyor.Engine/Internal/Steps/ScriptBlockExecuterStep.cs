﻿using LocalAppVeyor.Engine.Configuration;
using LocalAppVeyor.Engine.IO;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal abstract class ScriptBlockExecuterStep : IInternalEngineStep
    {
        private readonly ScriptBlock scriptBlock;

        private readonly FileSystem fileSystem;

        private readonly string workingDirectory;

        protected ScriptBlockExecuterStep(
            FileSystem fileSystem, 
            string workingDirectory,
            ScriptBlock scriptBlock)
        {
            this.fileSystem = fileSystem;
            this.scriptBlock = scriptBlock;
            this.workingDirectory = workingDirectory;
        }

        public bool Execute(ExecutionContext executionContext)
        {
            if (scriptBlock != null)
            {
                foreach (var scriptLine in scriptBlock)
                {
                    bool status;
                    if (string.IsNullOrEmpty(scriptLine.Script))
                    {
                        return true;
                    }

                    switch (scriptLine.ScriptType)
                    {
                        case ScriptType.Batch:
                            status = ExecuteBatchScript(executionContext, scriptLine.Script);
                            break;
                        case ScriptType.PowerShell:
                            status = ExecutePowerShellScript(executionContext, scriptLine.Script);
                            break;
                        default:
                            status = false;
                            break;
                    }
                    if (!status)
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
                fileSystem,
                workingDirectory,
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
                workingDirectory,
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
