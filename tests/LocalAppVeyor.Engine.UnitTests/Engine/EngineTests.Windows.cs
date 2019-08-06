using System.IO;
using System.IO.Abstractions;
using LocalAppVeyor.Engine.Configuration;
using LocalAppVeyor.Engine.UnitTests.TestUtilities;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace LocalAppVeyor.Engine.UnitTests.Engine
{
    public class EngineTests
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly Mock<IPipelineOutputter> _outputterMock = new Mock<IPipelineOutputter>();
        private readonly EngineConfiguration _engineConfiguration;

        public EngineTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            _engineConfiguration = new EngineConfiguration(Directory.GetCurrentDirectory(), _outputterMock.Object, new FileSystem());
        }

        [WindowsOnlyFact]
        public void ShouldRunInitializationPowershellScript()
        {
            var buildConfiguration = new BuildConfiguration
            {
                InitializationScript =
                {
                    new ScriptLine(ScriptType.PowerShell, "Write-Host 'This is a test'")
                }
            };

            var jobResult = new LocalAppVeyor.Engine.Engine(_engineConfiguration, buildConfiguration).ExecuteJob(0);

            _outputterMock.Verify(outputter => outputter.Write(It.Is<string>(m => m == "This is a test")), Times.Once);
            Assert.True(jobResult.IsSuccessfulExecution);
        }

        [WindowsOnlyFact]
        public void ShouldRunInitializationPowershellScriptWithMultipleLines()
        {
            var buildConfiguration = new BuildConfiguration
            {
                InitializationScript =
                {
                    new ScriptLine(ScriptType.PowerShell, "Write-Host 'This is a test - first line'"),
                    new ScriptLine(ScriptType.PowerShell, "Write-Host 'This is a test - second line'")
                }
            };

            var jobResult = new LocalAppVeyor.Engine.Engine(_engineConfiguration, buildConfiguration).ExecuteJob(0);

            _outputterMock.Verify(outputter => outputter.Write(It.Is<string>(m => m == "This is a test - first line")), Times.Once);
            _outputterMock.Verify(outputter => outputter.Write(It.Is<string>(m => m == "This is a test - second line")), Times.Once);
            _outputterMock.Verify(outputter => outputter.WriteError(It.IsAny<string>()), Times.Never);
            Assert.True(jobResult.IsSuccessfulExecution);
        }

        [WindowsOnlyFact]
        public void ShouldRunInitializationBatchScript()
        {
            var buildConfiguration = new BuildConfiguration
            {
                InitializationScript =
                {
                    new ScriptLine(ScriptType.Batch, "echo This is a test")
                }
            };

            var jobResult = new LocalAppVeyor.Engine.Engine(_engineConfiguration, buildConfiguration).ExecuteJob(0);

            _outputterMock.Verify(outputter => outputter.Write(It.Is<string>(m => m == "This is a test")), Times.Once);
            _outputterMock.Verify(outputter => outputter.WriteError(It.IsAny<string>()), Times.Never);
            Assert.True(jobResult.IsSuccessfulExecution);
        }

        [WindowsOnlyFact]
        public void ShouldRunInitializationBatchScriptWithMultipleLines()
        {
            var buildConfiguration = new BuildConfiguration
            {
                InitializationScript =
                {
                    new ScriptLine(ScriptType.Batch, "echo This is a test - first line"),
                    new ScriptLine(ScriptType.Batch, "echo This is a test - second line")
                }
            };

            var jobResult = new LocalAppVeyor.Engine.Engine(_engineConfiguration, buildConfiguration).ExecuteJob(0);

            _outputterMock.Verify(outputter => outputter.Write(It.Is<string>(m => m == "This is a test - first line")), Times.Once);
            _outputterMock.Verify(outputter => outputter.Write(It.Is<string>(m => m == "This is a test - second line")), Times.Once);
            _outputterMock.Verify(outputter => outputter.WriteError(It.IsAny<string>()), Times.Never);
            Assert.True(jobResult.IsSuccessfulExecution);
        }

        [WindowsOnlyFact]
        public void ShouldRunInitializationScriptWithMixedPowershellAndBatchScripts()
        {
            var buildConfiguration = new BuildConfiguration
            {
                InitializationScript =
                {
                    new ScriptLine(ScriptType.Batch, "echo This is 1 batch test"),
                    new ScriptLine(ScriptType.PowerShell, "Write-Host 'This is 1 powershell test'"),
                    new ScriptLine(ScriptType.Batch, "echo This is 2 batch test"),
                    new ScriptLine(ScriptType.PowerShell, "Write-Host 'This is 2 powershell test'"),
                    new ScriptLine(ScriptType.PowerShell, "Write-Host 'This is 3 powershell test'"),
                }
            };

            var jobResult = new LocalAppVeyor.Engine.Engine(_engineConfiguration, buildConfiguration).ExecuteJob(0);

            _outputHelper.WriteLine("UNHANDLED Current: " + File.Exists(Directory.GetCurrentDirectory()));
            _outputHelper.WriteLine("UNHANDLED Current: " + Directory.GetCurrentDirectory());
            _outputHelper.WriteLine("UNHANDLED: " + jobResult?.UnhandledException?.Message);

            _outputterMock.Verify(outputter => outputter.Write(It.Is<string>(m => m == "This is 1 batch test")), Times.Once);
            _outputterMock.Verify(outputter => outputter.Write(It.Is<string>(m => m == "This is 1 powershell test")), Times.Once);
            _outputterMock.Verify(outputter => outputter.Write(It.Is<string>(m => m == "This is 2 batch test")), Times.Once);
            _outputterMock.Verify(outputter => outputter.Write(It.Is<string>(m => m == "This is 2 powershell test")), Times.Once);
            _outputterMock.Verify(outputter => outputter.Write(It.Is<string>(m => m == "This is 3 powershell test")), Times.Once);
            _outputterMock.Verify(outputter => outputter.WriteError(It.IsAny<string>()), Times.Never);
            Assert.True(jobResult.IsSuccessfulExecution);
        }

        [WindowsOnlyFact]
        public void ShouldRunInitializationScriptPowershellBlockScript()
        {
            var buildConfiguration = new BuildConfiguration
            {
                InitializationScript =
                {
                    new ScriptLine(ScriptType.PowerShell, @"
Write-Host 'This is line one'
Write-Host 'And this is line two'")
                }
            };

            var jobResult = new LocalAppVeyor.Engine.Engine(_engineConfiguration, buildConfiguration).ExecuteJob(0);

            _outputterMock.Verify(outputter => outputter.Write(It.Is<string>(m => m == "This is line one")), Times.Once);
            _outputterMock.Verify(outputter => outputter.Write(It.Is<string>(m => m == "And this is line two")), Times.Once);
            _outputterMock.Verify(outputter => outputter.WriteError(It.IsAny<string>()), Times.Never);
            Assert.True(jobResult.IsSuccessfulExecution);
        }
    }
}