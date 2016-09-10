using System.Collections.Generic;
using System.Linq;
using LocalAppVeyor.Configuration.Model;

namespace LocalAppVeyor.Configuration.Reader.Internal.Model
{
    internal class InternalScriptBlock : List<InternalScriptLine>
    {
        public ScriptBlock ToScriptBlock()
        {
            return new ScriptBlock(this.Select(l => l.ToScriptLine()));
        }
    }
}