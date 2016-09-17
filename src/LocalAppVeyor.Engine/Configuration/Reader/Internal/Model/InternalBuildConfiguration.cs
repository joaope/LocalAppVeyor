using YamlDotNet.Serialization;

namespace LocalAppVeyor.Engine.Configuration.Reader.Internal.Model
{
    internal class InternalBuildConfiguration
    {
        [YamlMember(Alias = "version")]
        public string Version { get; set; }

        [YamlMember(Alias = "init")]
        public InternalScriptBlock InitializationScript { get; set; }

        [YamlMember(Alias = "clone_folder")]
        public string CloneFolder { get; set; }

        [YamlMember(Alias = "os")]
        public InternalOperatingSystems OperatingSystems { get; set; }

        [YamlMember(Alias = "environment")]
        public InternalEnvironmentVariables EnvironmentVariables { get; set; }

        [YamlMember(Alias = "matrix")]
        public InternalMatrix Matrix { get; set; }

        [YamlMember(Alias = "install")]
        public InternalScriptBlock InstallScript { get; set; }

        [YamlMember(Alias = "assembly_info")]
        public InternalAssemblyVersion AssemblyVersion { get; set; }

        [YamlMember(Alias = "platform")]
        public InternalPlatforms Platforms { get; set; }

        [YamlMember(Alias = "configuration")]
        public InternalConfigurations Configurations { get; set; }

        [YamlMember(Alias = "build")]
        public InternalBuild Build { get; set; }

        [YamlMember(Alias = "before_build")]
        public InternalScriptBlock BeforeBuildScript { get; set; }

        [YamlMember(Alias = "after_build")]
        public InternalScriptBlock AfterBuildScript { get; set; }

        [YamlMember(Alias = "build_script")]
        public InternalScriptBlock BuildScript { get; set; }

        [YamlMember(Alias = "test_script")]
        public InternalScriptBlock TestScript { get; set; }

        [YamlMember(Alias = "on_success")]
        public InternalScriptBlock OnSuccessScript { get; set; }

        [YamlMember(Alias = "on_failure")]
        public InternalScriptBlock OnFailureScript { get; set; }

        [YamlMember(Alias = "on_finish")]
        public InternalScriptBlock OnFinishScript { get; set; }

        public BuildConfiguration ToBuildConfiguration()
        {
            return new BuildConfiguration(
                Version,
                InitializationScript?.ToScriptBlock(),
                CloneFolder,
                InstallScript?.ToScriptBlock(),
                AssemblyVersion?.ToAssemblyInfo(),
                OperatingSystems,
                EnvironmentVariables?.ToEnvironmentVariables(),
                Matrix?.ToMatrix(),
                Platforms,
                Configurations,
                Build?.ToBuild(),
                BeforeBuildScript?.ToScriptBlock(),
                BuildScript?.ToScriptBlock(),
                AfterBuildScript?.ToScriptBlock(),
                TestScript?.ToScriptBlock(),
                OnSuccessScript?.ToScriptBlock(),
                OnFailureScript?.ToScriptBlock(),
                OnFinishScript?.ToScriptBlock());
        }
    }
}
