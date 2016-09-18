namespace LocalAppVeyor.Engine.Configuration
{
    public class AssemblyInfo
    {
        public bool Patch { get; }

        public ExpandableString File { get; }

        public ExpandableString AssemblyVersion { get; }

        public ExpandableString AssemblyFileVersion { get; }

        public ExpandableString AssemblyInformationalVersion { get; }

        public AssemblyInfo()
            : this(
                  false,
                  null,
                  null,
                  null,
                  null)
        {
        }
        
        public AssemblyInfo(
            bool patch,
            ExpandableString file,
            ExpandableString assemblyVersion,
            ExpandableString assemblyFileVersion,
            ExpandableString assemblyInformationalVersion)
        {
            Patch = patch;
            File = file;
            AssemblyVersion = assemblyVersion;
            AssemblyFileVersion = assemblyFileVersion;
            AssemblyInformationalVersion = assemblyInformationalVersion;
        }
    }
}
