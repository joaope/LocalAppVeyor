using System;
using System.Collections.Generic;
using FluentAssertions;
using LocalAppVeyor.Engine.Configuration;
using LocalAppVeyor.Engine.Configuration.Reader;
using LocalAppVeyor.Engine.Internal;
using LocalAppVeyor.Engine.Internal.Steps;
using LocalAppVeyor.Engine.IO;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace LocalAppVeyor.Engine.UnitTests.Engine
{
    public class AssemblyInfoRewriteStepTests : BaseTestClass
    {
        public AssemblyInfoRewriteStepTests(ITestOutputHelper outputter)
            : base(outputter)
        {
        }

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

            var executionContext = new ExecutionContext(
                new MatrixJob("os", new List<Variable>(), "conf", "platform"), 
                new BuildConfigurationYamlStringReader(yaml).GetBuildConfiguration(),
                new Mock<IPipelineOutputter>().Object,
                @"c:\repository\directory",
                @"c:\clone\directory");

            var mockedDirectoryHandler = new Mock<DirectoryHandler>();
            var mockedFileHandler = new Mock<FileHandler>();

            mockedDirectoryHandler
                .Setup(
                    d => d.EnumerateFiles(
                        executionContext.CloneDirectory,
                        executionContext.BuildConfiguration.AssemblyInfo.File, 
                        SearchOption.AllDirectories))
                .Returns(new List<string> { "AssemblyInfo.cs" });

            mockedFileHandler
                .Setup(f => f.ReadAllText("AssemblyInfo.cs"))
                .Returns(originalAssemblyInfoContent);

            Environment.SetEnvironmentVariable("APPVEYOR_BUILD_VERSION", new ExpandableString("2.3.{build}"));

            var rewriteStep = new AssemblyInfoRewriteStep(
                new FileSystem(mockedDirectoryHandler.Object, mockedFileHandler.Object, Mock.Of<PathHandler>()));

            var executionResult = rewriteStep.Execute(executionContext);

            mockedFileHandler.Verify(
                f => f.ReadAllText(It.Is<string>(s => s == "AssemblyInfo.cs")),
                Times.Once);

            mockedFileHandler.Verify(
                f => f.WriteAllText(It.Is<string>(s => s == "AssemblyInfo.cs"), It.Is<string>(s => s == rewrittenAssemblyInfoContent)), 
                Times.Once);

            executionResult.Should().BeTrue();
        }
    }
}

