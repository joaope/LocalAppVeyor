using LocalAppVeyor.Engine.Internal;
using YamlDotNet.Serialization;

namespace LocalAppVeyor.Engine.Configuration.Reader.Internal.Model
{
    internal class InternalScriptLine
    {
        [YamlMember(Alias = "cmd")]
        public string Batch { get; set; }

        [YamlMember(Alias = "ps")]
        public string PowerShell { get; set; }

        [YamlMember(Alias = "sh")]
        public string Bash { get; set; }

        public static implicit operator InternalScriptLine(string scriptLine)
        {
            if (Platform.IsUnix)
            {
                return new InternalScriptLine
                {
                    Bash = scriptLine
                };
            }

            return new InternalScriptLine
            {
                Batch = scriptLine
            };
        }

        public ScriptLine ToScriptLine()
        {
            var scriptType = string.IsNullOrEmpty(PowerShell)
                ? string.IsNullOrEmpty(Batch)
                    ? ScriptType.Bash
                    : ScriptType.Batch
                : ScriptType.PowerShell;

            return new ScriptLine(
                scriptType,
                scriptType == ScriptType.PowerShell
                    ? PowerShell
                    : scriptType == ScriptType.Batch
                        ? Batch
                        : Bash);
        }
    }
}