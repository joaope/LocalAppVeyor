using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FluentAssertions;
using LocalAppVeyor.Engine.Configuration;
using LocalAppVeyor.Engine.Configuration.Reader;
using Xunit;

namespace LocalAppVeyor.Engine.UnitTests.Configuration
{
    public class EnvironmentTests
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

            conf.EnvironmentVariables.Should().BeEquivalentTo(
                new EnvironmentVariables(
                    new List<Variable>
                    {
                        new Variable("common_var1", "common_value1", false),
                        new Variable("common_var2", "common_value2_secured", true)
                    },
                    new List<ReadOnlyCollection<Variable>>
                    {
                        new ReadOnlyCollection<Variable>(new List<Variable>
                        {
                            new Variable("db", "mysql", false),
                            new Variable("password", "mysql_password", false)
                        }),
                        new ReadOnlyCollection<Variable>(new List<Variable>
                        {
                            new Variable("db", "sqlserver", false),
                            new Variable("password", "sqlserver_secured_password", true)
                        })
                    }));
        }

        [Fact]
        public void ShouldReadCommonVariablesInsideGlobal()
        {
            const string yaml = @"
environment:
  global:
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

            conf.EnvironmentVariables.Should().BeEquivalentTo(
                new EnvironmentVariables(
                    new List<Variable>
                    {
                        new Variable("common_var1", "common_value1", false),
                        new Variable("common_var2", "common_value2_secured", true)
                    },
                    new List<ReadOnlyCollection<Variable>>
                    {
                        new ReadOnlyCollection<Variable>(new List<Variable>
                        {
                            new Variable("db", "mysql", false),
                            new Variable("password", "mysql_password", false)
                        }),
                        new ReadOnlyCollection<Variable>(new List<Variable>
                        {
                            new Variable("db", "sqlserver", false),
                            new Variable("password", "sqlserver_secured_password", true)
                        })
                    }));
        }

        [Fact]
        public void ShouldReadOnlyMatrixVariables()
        {
            const string yaml = @"
environment:
  matrix:
    - db: mysql
      password: mysql_password
    - db: sqlserver
      password:
        secure: sqlserver_secured_password
";

            var conf = new BuildConfigurationYamlStringReader(yaml).GetBuildConfiguration();

            conf.EnvironmentVariables.CommonVariables.Should().BeEmpty();
            conf.EnvironmentVariables.Should().BeEquivalentTo(
                new EnvironmentVariables(
                    null,
                    new List<ReadOnlyCollection<Variable>>
                    {
                        new ReadOnlyCollection<Variable>(new List<Variable>
                        {
                            new Variable("db", "mysql", false),
                            new Variable("password", "mysql_password", false)
                        }),
                        new ReadOnlyCollection<Variable>(new List<Variable>
                        {
                            new Variable("db", "sqlserver", false),
                            new Variable("password", "sqlserver_secured_password", true)
                        })
                    }));
        }

        [Fact]
        public void ShouldReadNoEnvironmentButPropertiesShouldNotBeNullOnlyEmpty()
        {
            var conf = new BuildConfigurationYamlStringReader("").GetBuildConfiguration();

            conf.EnvironmentVariables.Should().NotBeNull();
            conf.EnvironmentVariables.CommonVariables.Should().BeEmpty();
            conf.EnvironmentVariables.Matrix.Should().BeEmpty();
        }

        [Fact]
        public void ShouldExpandVariableValueWhenUsed()
        {
            Environment.SetEnvironmentVariable("ENV_VAR", "my env value");

            const string yaml = @"
environment:
  common_var1: common_value1 $(ENV_VAR)
";

            var conf = new BuildConfigurationYamlStringReader(yaml).GetBuildConfiguration();

            conf.EnvironmentVariables.Should().BeEquivalentTo(
                new EnvironmentVariables(
                    new List<Variable>
                    {
                        new Variable("common_var1", "common_value1 $(ENV_VAR)", false)
                    }));
            conf.EnvironmentVariables.CommonVariables[0].Value.Should().Be("common_value1 my env value");
            conf.EnvironmentVariables.Matrix.Should().BeEmpty();
        }
    }
}
