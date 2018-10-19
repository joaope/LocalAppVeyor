using FluentAssertions;
using LocalAppVeyor.Engine.Configuration;
using LocalAppVeyor.Engine.Configuration.Reader;
using Xunit;

namespace LocalAppVeyor.Engine.UnitTests.Configuration
{
    public class AssemblyInfoTests
    {
        [Fact]
        public void ShouldReadAssemblyInfoStep()
        {
            const string yaml = @"
assembly_info:
  patch: true
  file: AssemblyInfo.*
  assembly_version: ""2.2.{build}""
  assembly_file_version: ""{version}""
  assembly_informational_version: ""{version}""
";

            var conf = new BuildConfigurationYamlStringReader(yaml).GetBuildConfiguration();

            conf.AssemblyInfo.Should().BeEquivalentTo(new AssemblyInfo(
                true,
                "AssemblyInfo.*",
                "2.2.{build}",
                "{version}",
                "{version}"));
        }

        [Fact]
        public void ShouldBePatchFalseForAssemblyInfoWhenNotSpecified()
        {
            var conf = new BuildConfigurationYamlStringReader(string.Empty).GetBuildConfiguration();

            conf.AssemblyInfo.Should().BeEquivalentTo(new AssemblyInfo());
            conf.AssemblyInfo.Should().BeEquivalentTo(new AssemblyInfo(
                false,
                null,
                null,
                null,
                null));
        }
    }
}