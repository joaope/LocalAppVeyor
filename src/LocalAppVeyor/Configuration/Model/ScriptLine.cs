using YamlDotNet.Serialization;

namespace LocalAppVeyor.Configuration.Model
{
    public class ScriptLine
    {
        [YamlMember(Alias = "cmd")]
        public string Batch { get; internal set; }

        [YamlMember(Alias = "ps")]
        public string PowerShell { get; internal set; }

        public static implicit operator ScriptLine(string scriptLine)
        {
            return new ScriptLine
            {
                Batch = scriptLine
            };
        }
    }
}