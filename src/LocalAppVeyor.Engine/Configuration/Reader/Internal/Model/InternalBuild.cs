using YamlDotNet.Serialization;

namespace LocalAppVeyor.Engine.Configuration.Reader.Internal.Model
{
    internal class InternalBuild
    {
        [YamlIgnore]
        public bool IsAutomaticBuildOff { get; set; }

        [YamlMember(Alias = "parallel")]
        public bool IsParallel { get; set; }

        [YamlMember(Alias = "project")]
        public string SolutionFile { get; set; }
        
        public InternalBuildVerbosity? Verbosity { get; set; }

        public static implicit operator InternalBuild(string offString)
        {
            if (!string.IsNullOrEmpty(offString) && offString == "off")
            {
                return new InternalBuild
                {
                    IsAutomaticBuildOff = true
                };
            }

            return null;
        }

        public Build ToBuild()
        {
            return new Build(
                IsAutomaticBuildOff,
                IsParallel,
                SolutionFile,
                Verbosity?.ToBuildVerbosity() ?? BuildVerbosity.Normal);
        }
    }
}
