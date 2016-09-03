using YamlDotNet.Serialization;

namespace LocalAppVeyor.Configuration.Model
{
    public class BuildConfiguration
    {
        public string Version { get; internal set; }

        [YamlMember(Alias = "init")]
        public virtual ScriptBlock InitializationScript { get; internal set; }

        [YamlMember(Alias = "clone_folder")]
        public virtual string CloneFolder { get; internal set; }

        [YamlMember(Alias = "os")]
        public virtual OperatingSystems OperatingSystems { get; internal set; }

        [YamlMember(Alias = "environment")]
        public virtual EnvironmentVariables EnvironmentVariables { get; internal set; }

        [YamlMember(Alias = "install")]
        public virtual ScriptBlock InstallScript { get; internal set; }

        [YamlMember(Alias = "platform")]
        public virtual Platforms Platforms { get; internal set; }

        [YamlMember(Alias = "configuration")]
        public virtual Configurations Configurations { get; internal set; }

        public virtual Build Build { get; internal set; }

        [YamlMember(Alias = "before_build")]
        public virtual ScriptBlock BeforeBuildScript { get; internal set; }

        [YamlMember(Alias = "after_build")]
        public virtual ScriptBlock AfterBuildScript { get; internal set; }

        [YamlMember(Alias = "build_script")]
        public virtual ScriptBlock BuildScript { get; internal set; }


    }
}
