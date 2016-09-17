namespace LocalAppVeyor.Engine.Configuration
{
    public class ScriptLine
    {
        public ScriptType ScriptType { get; }

        public string Script { get; }

        public ScriptLine(ScriptType scriptType, string script)
        {
            ScriptType = scriptType;
            Script = script;
        }
    }
}