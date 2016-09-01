using YamlDotNet.Serialization;

namespace LocalAppVeyor.Configuration.Model
{
    public class AssemblyInfoPatching
    {
        [YamlMember(Alias = "patch")]
        public bool IsPatchingEnabled { get; set; }

        public string File { get; set; }

        [YamlMember(Alias = "assembly_version")]
        public string AssemblyVersion { get; set; }

        [YamlMember(Alias = "assembly_file_version")]
        public string AssemblyFileVersion { get; set; }

        [YamlMember(Alias = "assembly_informational_version")]
        public string AssemblyInformationalVersion { get; set; }
    }
}
