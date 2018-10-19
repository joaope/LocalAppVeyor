using System;
using FluentAssertions;
using LocalAppVeyor.Engine.Configuration;
using LocalAppVeyor.Engine.Configuration.Reader;
using Xunit;

namespace LocalAppVeyor.Engine.UnitTests.Configuration
{
    public class ExpandableStringTests
    {
        [Fact]
        public void ExpandEnvironmentVariablesOnString()
        {
            ExpandableString str = "this a string $(VAR1) and also $(VAR2)";

            Environment.SetEnvironmentVariable("VAR1", "VAR1_VALUE1");
            Environment.SetEnvironmentVariable("VAR2", "VAR2_VALUE2");

            str.Should().Be("this a string VAR1_VALUE1 and also VAR2_VALUE2");
        }

        [Fact]
        public void DontExpandVariablesWhenOpenButNotClosingBrace()
        {
            ExpandableString str = "this a string $(VAR1 and also $(VAR2";

            Environment.SetEnvironmentVariable("VAR1", "VAR1_VALUE1");
            Environment.SetEnvironmentVariable("VAR2", "VAR2_VALUE2");

            str.Should().Be("this a string $(VAR1 and also $(VAR2");
        }

        [Fact]
        public void DontExpandVariablesWithInvalidCharacters()
        {
            ExpandableString str = "this a string $(VAR 1) and also $(VAR2)";

            Environment.SetEnvironmentVariable("VAR1", "VAR1_VALUE1");
            Environment.SetEnvironmentVariable("VAR2", "VAR2_VALUE2");

            str.Should().Be("this a string $(VAR 1) and also VAR2_VALUE2");
        }

        [Fact]
        public void Expand()
        {
            ExpandableString str = "this a string $(VAR1) and also $(VAR2)";

            Environment.SetEnvironmentVariable("VAR1", "VAR1_VALUE1");
            Environment.SetEnvironmentVariable("VAR2", "VAR2_VALUE2");

            str.Should().Be("this a string VAR1_VALUE1 and also VAR2_VALUE2");
        }

        [Fact]
        public void ShouldExpandCloneFolderWithVersionAndBuildNumber()
        {
            Environment.SetEnvironmentVariable("APPVEYOR_BUILD_BUILD", "0");
            Environment.SetEnvironmentVariable("APPVEYOR_BUILD_VERSION", "1.0.0-0");

            const string yaml = @"
version: 1.0.0-{build}
clone_folder: c:\folder\with\version_{version}_here
";

            var conf = new BuildConfigurationYamlStringReader(yaml).GetBuildConfiguration();

            conf.Version.Should().Be("1.0.0-0");
            conf.CloneFolder.Should().Be(@"c:\folder\with\version_1.0.0-0_here");
        }
    }
}
