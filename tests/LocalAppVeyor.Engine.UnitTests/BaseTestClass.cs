using LocalAppVeyor.Engine.IO;
using Moq;
using Xunit.Abstractions;

namespace LocalAppVeyor.Engine.UnitTests
{
    public abstract class BaseTestClass
    {
        private readonly ITestOutputHelper outputter;

        protected BaseTestClass(ITestOutputHelper outputter)
        {
            this.outputter = outputter;
        }

        //protected static Mock<FileSystem> GetNewMockedFileSystem()
        //{

        //}
    }
}
