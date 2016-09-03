using YamlDotNet.Serialization;

namespace LocalAppVeyor.Configuration.Model
{
    public class AssemblyInfoPatching
    {
        [YamlMember(Alias = "patch")]
        public bool IsPatchingEnabled { get; internal set; }

        public string File { get; internal set; }

        [YamlMember(Alias = "assembly_version")]
        public string AssemblyVersion { get; internal set; }

        [YamlMember(Alias = "assembly_file_version")]
        public string AssemblyFileVersion { get; internal set; }

        [YamlMember(Alias = "assembly_informational_version")]
        public string AssemblyInformationalVersion { get; internal set; }
    }
}
