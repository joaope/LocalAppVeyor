using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Moq;
using Xunit.Abstractions;

namespace LocalAppVeyor.Engine.UnitTests.Engine
{
    public partial class EngineTests
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly Mock<IPipelineOutputter> _outputterMock = new Mock<IPipelineOutputter>();
        private readonly EngineConfiguration _engineConfiguration;
        private readonly IFileSystem _fileSystem = new MockFileSystem();

        public EngineTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            _engineConfiguration = new EngineConfiguration(_fileSystem.Directory.GetCurrentDirectory(), _outputterMock.Object, _fileSystem);
        }
    }
}