using YamlDotNet.Serialization;

namespace LocalAppVeyor.Configuration.Model
{
    public class Build
    {
        [YamlIgnore]
        public bool IsAutomaticBuildOff { get; set; }

        [YamlMember(Alias = "parallel")]
        public bool IsParallel { get; set; }

        [YamlMember(Alias = "project")]
        public string SolutionFile { get; set; }
        
        public BuildVerbosity? Verbosity { get; set; }

        public static implicit operator Build(string offString)
        {
            if (!string.IsNullOrEmpty(offString) && offString == "off")
            {
                return new Build
                {
                    IsAutomaticBuildOff = true
                };
            }

            return null;
        }
    }
}
