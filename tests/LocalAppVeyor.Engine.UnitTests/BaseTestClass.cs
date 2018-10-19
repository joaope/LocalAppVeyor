using Xunit.Abstractions;

namespace LocalAppVeyor.Engine.UnitTests
{
    public abstract class BaseTestClass
    {
        private readonly ITestOutputHelper _outputter;

        protected BaseTestClass(ITestOutputHelper outputter)
        {
            _outputter = outputter;
        }
    }
}
