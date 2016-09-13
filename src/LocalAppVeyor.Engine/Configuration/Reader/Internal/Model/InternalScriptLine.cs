using YamlDotNet.Serialization;

namespace LocalAppVeyor.Engine.Configuration.Reader.Internal.Model
{
    internal class InternalScriptLine
    {
        [YamlMember(Alias = "cmd")]
        public string Batch { get; set; }

        [YamlMember(Alias = "ps")]
        public string PowerShell { get; set; }

        public static implicit operator InternalScriptLine(string scriptLine)
        {
            return new InternalScriptLine
            {
                Batch = scriptLine
            };
        }

        public ScriptLine ToScriptLine()
        {
            return new ScriptLine(
                string.IsNullOrEmpty(PowerShell) ? ScriptType.Batch : ScriptType.PowerShell,
                string.IsNullOrEmpty(PowerShell) ? Batch : PowerShell);
        }
    }
}