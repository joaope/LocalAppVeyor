namespace LocalAppVeyor.Engine.Configuration
{
    public class ScriptLine
    {
        public ScriptType ScriptType { get; set; }

        public string Script { get; set; }

        public ScriptLine(ScriptType scriptType, string script)
        {
            ScriptType = scriptType;
            Script = script;
        }
    }
}