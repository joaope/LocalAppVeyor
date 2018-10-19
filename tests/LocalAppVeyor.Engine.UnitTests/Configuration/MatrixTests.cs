using FluentAssertions;
using LocalAppVeyor.Engine.Configuration;
using LocalAppVeyor.Engine.Configuration.Reader;
using Xunit;

namespace LocalAppVeyor.Engine.UnitTests.Configuration
{
    public class MatrixTests
    {
        [Fact]
        public void MatrixShouldReadAllowFailuresSectionSuccessfully()
        {
            const string yaml = @"
matrix:
  allow_failures:
    - platform: x86
      configuration: Debug
    - platform: x64
      configuration: Release
";

            var conf = new BuildConfigurationYamlStringReader(yaml).GetBuildConfiguration();

            conf.Matrix.Should().BeEquivalentTo(new Matrix(
                false,
                new[]
                {
                    new AllowedJobFailureConditions(null, "Debug", "x86", null, new Variable[0]),
                    new AllowedJobFailureConditions(null, "Release", "x64", null, new Variable[0])
                }));

            conf.Matrix.AllowedFailures.Should().HaveCount(2);
        }

        [Fact]
        public void AllowFailuresSectionShouldBeEmpty()
        {
            const string yaml = @"
matrix:
";

            var conf = new BuildConfigurationYamlStringReader(yaml).GetBuildConfiguration();

            conf.Matrix.Should().BeEquivalentTo(new Matrix(
                false,
                new AllowedJobFailureConditions[0]));

            conf.Matrix.IsFastFinish.Should().BeFalse();
            conf.Matrix.AllowedFailures.Should().BeEmpty();
        }

        [Fact]
        public void MatrixShouldHaveDefaultValuesWhenNotSpecified()
        {
            var conf = new BuildConfigurationYamlStringReader(string.Empty).GetBuildConfiguration();

            conf.Matrix.Should().BeEquivalentTo(new Matrix());
            conf.Matrix.IsFastFinish.Should().BeFalse();
            conf.Matrix.AllowedFailures.Should().BeEmpty();
        }

        [Fact]
        public void MatrixShouldHaveFastFinishAsTrueAndEmptyAllowedFailures()
        {
            const string yaml = @"
matrix:
  fast_finish: true
";
            var conf = new BuildConfigurationYamlStringReader(yaml).GetBuildConfiguration();

            conf.Matrix.Should().BeEquivalentTo(new Matrix(
                true,
                new AllowedJobFailureConditions[0]));
            conf.Matrix.IsFastFinish.Should().BeTrue();
            conf.Matrix.AllowedFailures.Should().BeEmpty();
        }
    }
}
