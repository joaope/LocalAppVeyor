using System;
using System.Collections.Generic;
using System.Linq;

namespace LocalAppVeyor.Pipeline
{
    public class EngineConfiguration
    {
        public IEngineStep[] Steps { get; set; }

        public static EngineConfiguration Default => new EngineConfiguration(new IEngineStep[0]);

        public EngineConfiguration(IEnumerable<IEngineStep> steps)
        {
            if (steps == null) throw new ArgumentNullException(nameof(steps));

            Steps = steps.ToArray();
        }
    }
}
