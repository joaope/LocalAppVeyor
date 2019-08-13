using LocalAppVeyor.Engine.Configuration;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal abstract class ScriptBlockExecuterStep : IEngineStep
    {
        public abstract string Name { get; }

        private readonly ScriptBlock _scriptBlock;

        private readonly string _workingDirectory;

        protected ScriptBlockExecuterStep(
            string workingDirectory,
            ScriptBlock scriptBlock)
        {
            _scriptBlock = scriptBlock;
            _workingDirectory = workingDirectory;
        }

        public bool Execute(ExecutionContext executionContext)
        {
            if (_scriptBlock != null)
            {
                foreach (var scriptLine in _scriptBlock)
                {
                    if (string.IsNullOrEmpty(scriptLine.Script))
                    {
                        continue;
                    }

                    if (scriptLine.ScriptType == ScriptType.Bash && Platform.IsWindow)
                    {
                        executionContext.Outputter.Write("Script line skipped - Bash (sh) scripts cannot be executed in Windows platforms.");
                        continue;
                    }

                    if (scriptLine.ScriptType == ScriptType.Batch && Platform.IsUnix)
                    {
                        executionContext.Outputter.Write("Script line skipped - Batch (cmd) scripts cannot be executed in Unix platforms.");
                        continue;
                    }

                    var status = true;

                    switch (scriptLine.ScriptType)
                    {
                        case ScriptType.Batch:
                            status = ExecuteBatchScript(executionContext, scriptLine.Script);
                            break;
                        case ScriptType.PowerShell:
                            status = ExecutePowerShellScript(executionContext, scriptLine.Script);
                            break;
                        case ScriptType.Bash:
                            status = ExecuteBashScript(executionContext, scriptLine.Script);
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
                executionContext.FileSystem,
                _workingDirectory,
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

        private static bool ExecuteBashScript(ExecutionContext executionContext, string script)
        {
            return BashScriptExecuter.Execute(
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
