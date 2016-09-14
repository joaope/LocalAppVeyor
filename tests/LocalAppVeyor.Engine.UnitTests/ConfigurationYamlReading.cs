using FluentAssertions;
using LocalAppVeyor.Engine.Configuration.Reader;
using Xunit;

namespace LocalAppVeyor.Engine.UnitTests
{
    public class ConfigurationYamlReading
    {
        [Fact]
        public void ShouldReadEnvironmentWithCommonAndMatrixVariables()
        {
            const string yaml = @"
environment:
  common_var1: common_value1
  common_var2:
    secure: common_value2_secured
  matrix:
    - db: mysql
      password: mysql_password
    - db: sqlserver
      password:
        secure: sqlserver_secured_password
";

            var conf = new BuildConfigurationYamlStringReader(yaml).GetBuildConfiguration();

            conf.EnvironmentVariables.Should().NotBeNull();
            conf.EnvironmentVariables.CommonVariables.Should().HaveCount(2);

            conf.EnvironmentVariables.CommonVariables[0].Name.Should().Be("common_var1");
            conf.EnvironmentVariables.CommonVariables[0].Value.Should().Be("common_value1");
            conf.EnvironmentVariables.CommonVariables[0].IsSecuredValue.Should().BeFalse();

            conf.EnvironmentVariables.CommonVariables[1].Name.Should().Be("common_var2");
            conf.EnvironmentVariables.CommonVariables[1].Value.Should().Be("common_value2_secured");
            conf.EnvironmentVariables.CommonVariables[1].IsSecuredValue.Should().BeTrue();

            conf.EnvironmentVariables.Matrix.Should().NotBeNull();
            conf.EnvironmentVariables.Matrix.Should().HaveCount(2);

            conf.EnvironmentVariables.Matrix[0].Should().HaveCount(2);
            conf.EnvironmentVariables.Matrix[0][0].Name.Should().Be("db");
            conf.EnvironmentVariables.Matrix[0][0].Value.Should().Be("mysql");
            conf.EnvironmentVariables.Matrix[0][0].IsSecuredValue.Should().BeFalse();
            conf.EnvironmentVariables.Matrix[0][1].Name.Should().Be("password");
            conf.EnvironmentVariables.Matrix[0][1].Value.Should().Be("mysql_password");
            conf.EnvironmentVariables.Matrix[0][1].IsSecuredValue.Should().BeFalse();

            conf.EnvironmentVariables.Matrix[1].Should().HaveCount(2);
            conf.EnvironmentVariables.Matrix[1][0].Name.Should().Be("db");
            conf.EnvironmentVariables.Matrix[1][0].Value.Should().Be("sqlserver");
            conf.EnvironmentVariables.Matrix[1][0].IsSecuredValue.Should().BeFalse();
            conf.EnvironmentVariables.Matrix[1][1].Name.Should().Be("password");
            conf.EnvironmentVariables.Matrix[1][1].Value.Should().Be("sqlserver_secured_password");
            conf.EnvironmentVariables.Matrix[1][1].IsSecuredValue.Should().BeTrue();
        }

        [Fact]
        public void ShouldReadNoEnvironmentButPropertiesShouldNotBeNullOnlyEmpty()
        {
            var conf = new BuildConfigurationYamlStringReader("").GetBuildConfiguration();

            conf.EnvironmentVariables.Should().NotBeNull();
            conf.EnvironmentVariables.CommonVariables.Should().BeEmpty();
            conf.EnvironmentVariables.Matrix.Should().BeEmpty();
        }
    }
}
