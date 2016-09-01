using LocalAppVeyor.Configuration.Model;
using LocalAppVeyor.Pipeline.Steps.Internal;

namespace LocalAppVeyor.Pipeline.Steps
{
    public abstract class ScriptBlockExecuterStep : Step
    {
        protected abstract ScriptBlock ScriptBlock { get; }

        protected abstract bool IncludeEnvironmentVariables { get; }

        protected ScriptBlockExecuterStep(BuildConfiguration buildConfiguration)
            : base(buildConfiguration)
        {
        }

        public override bool Execute(ExecutionContext executionContext)
        {
            if (ScriptBlock != null)
            {
                foreach (var script in ScriptBlock)
                {
                    var result = true;

                    if (!string.IsNullOrEmpty(script.Batch))
                    {
                        result = ExecuteBatchScript(executionContext, script.Batch);
                    }
                    else if (!string.IsNullOrEmpty(script.PowerShell))
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
                executionContext.WorkingDirectory,
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
                },
                IncludeEnvironmentVariables ? executionContext.EnvironmentVariables : null);
        }   
    }
}
