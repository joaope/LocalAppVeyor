using System.IO.Abstractions;
using LocalAppVeyor.Engine.Configuration;
using Moq;
using Xunit;

namespace LocalAppVeyor.Engine.UnitTests.Engine
{
    public partial class EngineTests
    {
        private readonly Mock<IPipelineOutputter> _outputterMock = new Mock<IPipelineOutputter>();
        private readonly EngineConfiguration _engineConfiguration;
        private readonly IFileSystem _fileSystem = new FileSystem();

        public EngineTests()
        {
            var currentDirectory =
                _fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), _fileSystem.Path.GetRandomFileName());

            if (!_fileSystem.Directory.Exists(currentDirectory))
            {
                _fileSystem.Directory.CreateDirectory(currentDirectory);
            }

            _fileSystem.Directory.SetCurrentDirectory(currentDirectory);

            _engineConfiguration = new EngineConfiguration(_fileSystem.Directory.GetCurrentDirectory(), _outputterMock.Object, _fileSystem);
        }

        [Fact]
        public void ShouldSkipSpecifiedBuildStep()
        {
            var buildConfiguration = new BuildConfiguration
            {
                InitializationScript =
                {
                    new ScriptLine(ScriptType.PowerShell, "Write-Host 'This is init step'")
                },
                InstallScript =
                {
                    new ScriptLine(ScriptType.PowerShell, "Write-Host 'This is install step'")
                },
                BuildScript =
                {
                    new ScriptLine(ScriptType.PowerShell, "Write-Host 'This is build step'")
                },
                SkipSteps = new [] { "install" }
            };

            var jobResult = new LocalAppVeyor.Engine.Engine(_engineConfiguration, buildConfiguration).ExecuteJob(0);

            _outputterMock.Verify(outputter => outputter.Write(It.Is<string>(m => m == "This is init step")), Times.Once);
            _outputterMock.Verify(outputter => outputter.Write(It.Is<string>(m => m == "This is install step")), Times.Never);
            _outputterMock.Verify(outputter => outputter.Write(It.Is<string>(m => m == "This is build step")), Times.Once);
            Assert.True(jobResult.IsSuccessfulExecution);
        }

        [Fact]
        public void ShouldSkipSpecifiedBuildSteps()
        {
            var buildConfiguration = new BuildConfiguration
            {
                InitializationScript =
                {
                    new ScriptLine(ScriptType.PowerShell, "Write-Host 'This is init step'")
                },
                InstallScript =
                {
                    new ScriptLine(ScriptType.PowerShell, "Write-Host 'This is install step'")
                },
                BuildScript =
                {
                    new ScriptLine(ScriptType.PowerShell, "Write-Host 'This is build step'")
                },
                SkipSteps = new[] { "install", "build_script" }
            };

            var jobResult = new LocalAppVeyor.Engine.Engine(_engineConfiguration, buildConfiguration).ExecuteJob(0);

            _outputterMock.Verify(outputter => outputter.Write(It.Is<string>(m => m == "This is init step")), Times.Once);
            _outputterMock.Verify(outputter => outputter.Write(It.Is<string>(m => m == "This is install step")), Times.Never);
            _outputterMock.Verify(outputter => outputter.Write(It.Is<string>(m => m == "This is build step")), Times.Never);
            Assert.True(jobResult.IsSuccessfulExecution);
        }
    }
}