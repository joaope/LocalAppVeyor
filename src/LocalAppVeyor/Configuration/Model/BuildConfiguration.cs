using YamlDotNet.Serialization;

namespace LocalAppVeyor.Configuration.Model
{
    public class BuildConfiguration
    {
        public string Version { get; set; }

        [YamlMember(Alias = "init")]
        public ScriptBlock InitializationScript { get; set; }

        [YamlMember(Alias = "clone_folder")]
        public string CloneFolder { get; set; }

        [YamlMember(Alias = "os")]
        public string OperatingSystem { get; set; }

        [YamlMember(Alias = "environment")]
        public EnvironmentVariables EnvironmentVariables { get; set; }

        [YamlMember(Alias = "install")]
        public ScriptBlock InstallScript { get; set; }

        //[YamlMember(Alias = "assembly_info")]
        //public AssemblyInfoPatching AssemblyInfoPatching { get; set; }

        [YamlMember(Alias = "platform")]
        public Platforms Platforms { get; set; }

        [YamlMember(Alias = "configuration")]
        public Configurations Configurations { get; set; }

        public Build Build { get; set; }

        [YamlMember(Alias = "before_build")]
        public ScriptBlock BeforeBuildScript { get; set; }

        [YamlMember(Alias = "after_build")]
        public ScriptBlock AfterBuildScript { get; set; }

        [YamlMember(Alias = "build_script")]
        public ScriptBlock BuildScript { get; set; }


    }
}
