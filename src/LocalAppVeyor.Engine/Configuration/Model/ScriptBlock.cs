using System.Collections.Generic;

namespace LocalAppVeyor.Configuration.Model
{
    public class ScriptBlock : List<ScriptLine>
    {
        public ScriptBlock()
            : this(new ScriptLine[0])
        {
        }

        public ScriptBlock(IEnumerable<ScriptLine> scriptLines)
            : base(scriptLines)
        {
        }
    }
}