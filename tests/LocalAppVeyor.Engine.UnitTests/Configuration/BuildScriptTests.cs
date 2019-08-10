using LocalAppVeyor.Engine.Configuration;
using LocalAppVeyor.Engine.Configuration.Reader;
using Xunit;

namespace LocalAppVeyor.Engine.UnitTests.Configuration
{
    public partial class BuildScriptTests
    {
        [Fact]
        public void ShouldReadBuildScriptAsAScriptBlockWithSplittedLinesScripts_AlternativeBlockStylePowershell()
        {
            const string yaml = @"
build_script:
  - ps: |-
      echo --------------------------------------------------------------------------------
      echo Build tinyformat
      mkdir build
      cd build
      cmake -G ""%COMPILER%"" ..
      cmake --build . --config %CONFIGURATION%
";

            var conf = new BuildConfigurationYamlStringReader(yaml).GetBuildConfiguration();

            Assert.Single(conf.BuildScript);
            Assert.Equal("echo --------------------------------------------------------------------------------\n" +
                         "echo Build tinyformat\n" +
                         "mkdir build\n" +
                         "cd build\n" +
                         "cmake -G \"%COMPILER%\" ..\n" +
                         "cmake --build . --config %CONFIGURATION%",
                conf.BuildScript[0].Script);
            Assert.Equal(ScriptType.PowerShell, conf.BuildScript[0].ScriptType);
        }
    }
}