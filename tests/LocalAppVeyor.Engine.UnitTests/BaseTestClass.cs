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
    }
}
