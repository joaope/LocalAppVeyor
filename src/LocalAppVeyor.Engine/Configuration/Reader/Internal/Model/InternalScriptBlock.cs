using System.Collections.Generic;
using System.Linq;
using LocalAppVeyor.Engine.Configuration.Model;

namespace LocalAppVeyor.Engine.Configuration.Reader.Internal.Model
{
    internal class InternalScriptBlock : List<InternalScriptLine>
    {
        public ScriptBlock ToScriptBlock()
        {
            return new ScriptBlock(this.Select(l => l.ToScriptLine()));
        }
    }
}