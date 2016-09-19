using System;
using System.IO;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal class AssemblyInfoRewriteStep : IInternalEngineStep
    {
        public bool Execute(ExecutionContext executionContext)
        {
            if (!executionContext.BuildConfiguration.AssemblyInfo.Patch)
            {
                return true;
            }

            if (string.IsNullOrEmpty(executionContext.BuildConfiguration.AssemblyInfo.File))
            {
                executionContext.Outputter.WriteWarning("No assembly info files specified.");
                return true;
            }

            foreach (var assemblyInfoFile in Directory.EnumerateFiles(
                executionContext.CloneDirectory,
                executionContext.BuildConfiguration.AssemblyInfo.File, 
                SearchOption.AllDirectories))
            {
                if (!RewriteAssemblyInfoFile(executionContext, assemblyInfoFile))
                {
                    return false;
                }
            }

            return true;
        }

        private bool RewriteAssemblyInfoFile(ExecutionContext executionContext, string filePath)
        {
            try
            {


                return true;
            }
            catch (Exception e)
            {
                executionContext.Outputter.WriteError($"Error re-writing '{filePath}': {e.Message}.");
                return false;
            }
        }
    }
}
