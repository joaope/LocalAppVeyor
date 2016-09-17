namespace LocalAppVeyor.Engine.Configuration
{
    public class AssemblyInfo
    {
        public bool Patch { get; }

        public string File { get; }

        public string AssemblyVersion { get; }

        public string AssemblyFileVersion { get; }

        public string AssemblyInformationalVersion { get; }

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
            string file, 
            string assemblyVersion, 
            string assemblyFileVersion, 
            string assemblyInformationalVersion)
        {
            Patch = patch;
            File = file;
            AssemblyVersion = assemblyVersion;
            AssemblyFileVersion = assemblyFileVersion;
            AssemblyInformationalVersion = assemblyInformationalVersion;
        }
    }
}
