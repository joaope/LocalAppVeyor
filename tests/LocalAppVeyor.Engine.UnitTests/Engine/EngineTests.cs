using System.IO;
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

        public EngineTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            _engineConfiguration = new EngineConfiguration(Directory.GetCurrentDirectory(), _outputterMock.Object, new FileSystem());
        }
    }
}