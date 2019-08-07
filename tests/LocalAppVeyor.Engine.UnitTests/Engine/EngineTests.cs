using System.IO.Abstractions;
using Moq;
using Xunit.Abstractions;

namespace LocalAppVeyor.Engine.UnitTests.Engine
{
    public partial class EngineTests
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly Mock<IPipelineOutputter> _outputterMock = new Mock<IPipelineOutputter>();
        private readonly EngineConfiguration _engineConfiguration;
        private readonly IFileSystem _fileSystem = new FileSystem();

        public EngineTests(ITestOutputHelper outputHelper)
        {
            var currentDirectory =
                _fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), _fileSystem.Path.GetRandomFileName());

            if (!_fileSystem.Directory.Exists(currentDirectory))
            {
                _fileSystem.Directory.CreateDirectory(currentDirectory);
            }

            _fileSystem.Directory.SetCurrentDirectory(currentDirectory);

            _outputHelper = outputHelper;
            _engineConfiguration = new EngineConfiguration(_fileSystem.Directory.GetCurrentDirectory(), _outputterMock.Object, _fileSystem);
        }
    }
}