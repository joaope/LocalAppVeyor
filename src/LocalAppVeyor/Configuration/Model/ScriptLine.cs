using YamlDotNet.Serialization;

namespace LocalAppVeyor.Configuration.Model
{
    public class ScriptLine
    {
        [YamlMember(Alias = "cmd")]
        public string Batch { get; set; }

        [YamlMember(Alias = "ps")]
        public string PowerShell { get; set; }

        public static implicit operator ScriptLine(string scriptLine)
        {
            return new ScriptLine
            {
                Batch = scriptLine
            };
        }
    }
}