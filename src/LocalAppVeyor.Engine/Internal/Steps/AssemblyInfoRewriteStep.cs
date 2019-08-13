using System.IO;
using System.Text.RegularExpressions;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal class AssemblyInfoRewriteStep : IEngineStep
    {
        public string Name => "assembly_info";

        private static readonly Regex AssemblyVersionPattern = new Regex(@"AssemblyVersion\("".+""\)", RegexOptions.Compiled);

        private static readonly Regex AssemblyFileVersionPattern = new Regex(@"AssemblyFileVersion\("".+""\)", RegexOptions.Compiled);

        private static readonly Regex AssemblyInformationalVersionPattern = new Regex(@"AssemblyInformationalVersion\("".+""\)", RegexOptions.Compiled);

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

            if (string.IsNullOrEmpty(executionContext.BuildConfiguration.AssemblyInfo.AssemblyFileVersion) &&
                string.IsNullOrEmpty(executionContext.BuildConfiguration.AssemblyInfo.AssemblyVersion) &&
                string.IsNullOrEmpty(executionContext.BuildConfiguration.AssemblyInfo.AssemblyInformationalVersion))
            {
                executionContext.Outputter.WriteWarning("No versioning information provided to re-write AssemlyInfo files with.");
                return false;
            }

            foreach (var assemblyInfoFile in executionContext.FileSystem.Directory.EnumerateFiles(
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
            executionContext.Outputter.Write($"Re-writing '{filePath}'...");

            var fileContent = executionContext.FileSystem.File.ReadAllText(filePath);

            if (!string.IsNullOrEmpty(executionContext.BuildConfiguration.AssemblyInfo.AssemblyVersion))
            {
                fileContent = AssemblyVersionPattern.Replace(
                    fileContent,
                    $@"AssemblyVersion(""{executionContext.BuildConfiguration.AssemblyInfo.AssemblyVersion}"")");
            }

            if (!string.IsNullOrEmpty(executionContext.BuildConfiguration.AssemblyInfo.AssemblyFileVersion))
            {
                fileContent = AssemblyFileVersionPattern.Replace(
                    fileContent,
                    $@"AssemblyFileVersion(""{executionContext.BuildConfiguration.AssemblyInfo.AssemblyFileVersion}"")");
            }

            if (!string.IsNullOrEmpty(executionContext.BuildConfiguration.AssemblyInfo.AssemblyInformationalVersion))
            {
                fileContent = AssemblyInformationalVersionPattern.Replace(
                    fileContent,
                    $@"AssemblyInformationalVersion(""{executionContext.BuildConfiguration.AssemblyInfo.AssemblyInformationalVersion}"")");
            }

            executionContext.FileSystem.File.WriteAllText(filePath, fileContent);

            return true;
        }
    }
}
