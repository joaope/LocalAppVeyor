using System.Collections.Generic;
using System.Linq;

namespace LocalAppVeyor.Engine.Configuration.Reader.Internal.Model
{
    internal class InternalScriptBlock : List<InternalScriptLine>
    {
        public ScriptBlock ToScriptBlock()
        {
            return new ScriptBlock(this.Select(l => l.ToScriptLine()));
        }

        public static implicit operator InternalScriptBlock(string _)
        {
            return new InternalScriptBlock();
        }
    }
}