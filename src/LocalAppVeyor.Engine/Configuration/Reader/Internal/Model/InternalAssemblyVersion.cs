using YamlDotNet.Serialization;

namespace LocalAppVeyor.Engine.Configuration.Reader.Internal.Model
{
    internal class InternalAssemblyVersion
    {
        [YamlMember(Alias = "patch")]
        public bool Patch { get; set; }

        [YamlMember(Alias = "file")]
        public string File { get; set; }

        [YamlMember(Alias = "assembly_version")]
        public string AssemblyVersion { get; set; }

        [YamlMember(Alias = "assembly_file_version")]
        public string AssemblyFileVersion { get; set; }

        [YamlMember(Alias = "assembly_informational_version")]
        public string AssemblyInformationalVersion { get; set; }

        public AssemblyInfo ToAssemblyInfo()
        {
            return new AssemblyInfo(
                Patch,
                File,
                AssemblyVersion,
                AssemblyFileVersion,
                AssemblyInformationalVersion);
        }
    }
}
