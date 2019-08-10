using LocalAppVeyor.Engine.Configuration;
using LocalAppVeyor.Engine.Configuration.Reader;
using LocalAppVeyor.Engine.UnitTests.TestUtilities;
using Xunit;

namespace LocalAppVeyor.Engine.UnitTests.Configuration
{
    public partial class BuildScriptTests
    {
        [UnixOnlyFact]
        public void Unix_ShouldReadBuildScriptAsAScriptBlockWithMultipleDifferentTypeScripts()
        {
            const string yaml = @"
build_script:
  # by default, all script lines are interpreted as bash
  - echo This is bash
  # to run script as a PowerShell command prepend it with ps:
  - ps: Write-Host 'This is PowerShell'
  # bash commands start from cmd:
  - cmd: echo This is bash again
  - cmd: set MY_VAR=12345
";

            var conf = new BuildConfigurationYamlStringReader(yaml).GetBuildConfiguration();

            Assert.Equal(4, conf.BuildScript.Count);
            Assert.Equal("echo This is bash", conf.BuildScript[0].Script);
            Assert.Equal(ScriptType.Bash, conf.BuildScript[0].ScriptType);
            Assert.Equal("Write-Host 'This is PowerShell'", conf.BuildScript[1].Script);
            Assert.Equal(ScriptType.PowerShell, conf.BuildScript[1].ScriptType);
            Assert.Equal("echo This is bash again", conf.BuildScript[2].Script);
            Assert.Equal(ScriptType.Batch, conf.BuildScript[2].ScriptType);
            Assert.Equal("set MY_VAR=12345", conf.BuildScript[3].Script);
            Assert.Equal(ScriptType.Batch, conf.BuildScript[3].ScriptType);
        }

        [UnixOnlyFact]
        public void Unix_ShouldReadBuildScriptAsAScriptBlockWithSplittedLinesScripts()
        {
            const string yaml = @"
build_script:
  - |-
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
            Assert.Equal(ScriptType.Bash, conf.BuildScript[0].ScriptType);
        }

        [UnixOnlyFact]
        public void Unix_ShouldReadBuildScriptAsAScriptBlockWithSplittedLinesScripts_AlternativeBlockStyle()
        {
            const string yaml = @"
build_script:
  - sh: |-
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
            Assert.Equal(ScriptType.Bash, conf.BuildScript[0].ScriptType);
        }
    }
}