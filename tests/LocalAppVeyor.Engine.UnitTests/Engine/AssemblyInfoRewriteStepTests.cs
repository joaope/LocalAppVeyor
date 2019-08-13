using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using LocalAppVeyor.Engine.Configuration;
using LocalAppVeyor.Engine.Configuration.Reader;
using LocalAppVeyor.Engine.Internal;
using LocalAppVeyor.Engine.Internal.Steps;
using Moq;
using Xunit;

namespace LocalAppVeyor.Engine.UnitTests.Engine
{
    public class AssemblyInfoRewriteStepTests
    {
        [Fact]
        public void AssemblyInfoReWriteShouldReplaceAllVersionAttributes()
        {
            const string yaml = @"
version: 2.3.{build}
assembly_info:
  patch: true
  file: AssemblyInfo.*
  assembly_version: ""1.1.{build}""
  assembly_file_version: ""{version}""
  assembly_informational_version: ""{version}""
";

            const string originalAssemblyInfoContent = @"
[assembly: AssemblyTitle(""PatchAssemblyInfoFiles"")]
[assembly: AssemblyDescription("""")]
[assembly: AssemblyVersion(""1.2.3.4"")]
[assembly: AssemblyFileVersion(""1.2.3.4"")]
[assembly: AssemblyInformationalVersion(""foo bar baz 1 2 3 4"")]
";

            const string rewrittenAssemblyInfoContent = @"
[assembly: AssemblyTitle(""PatchAssemblyInfoFiles"")]
[assembly: AssemblyDescription("""")]
[assembly: AssemblyVersion(""1.1.0"")]
[assembly: AssemblyFileVersion(""2.3.0"")]
[assembly: AssemblyInformationalVersion(""2.3.0"")]
";

            var fileSystem = new MockFileSystem();
            var cloneDirectory = fileSystem.Path.Combine(fileSystem.Path.GetTempPath(), fileSystem.Path.GetRandomFileName());
            var repoDirectory = fileSystem.Path.Combine(fileSystem.Path.GetTempPath(), fileSystem.Path.GetRandomFileName());
            var assemblyInfoFilename = fileSystem.Path.Combine(cloneDirectory, "AssemblyInfo.cs");
            fileSystem.AddDirectory(repoDirectory);
            fileSystem.AddDirectory(cloneDirectory);
            fileSystem.AddFile(assemblyInfoFilename, new MockFileData(originalAssemblyInfoContent));
            fileSystem.Directory.SetCurrentDirectory(repoDirectory);
            
            var executionContext = new ExecutionContext(
                new MatrixJob("os", new List<Variable>(), "conf", "platform"), 
                new BuildConfigurationYamlStringReader(yaml).GetBuildConfiguration(),
                new Mock<IPipelineOutputter>().Object,
                repoDirectory,
                cloneDirectory, 
                fileSystem);

            Environment.SetEnvironmentVariable("APPVEYOR_BUILD_VERSION", new ExpandableString("2.3.{build}"));

            var rewriteStep = new AssemblyInfoRewriteStep();
            var executionResult = rewriteStep.Execute(executionContext);

            Assert.True(executionResult);
            Assert.Equal(rewrittenAssemblyInfoContent, fileSystem.File.ReadAllText(assemblyInfoFilename));
        }
    }
}

