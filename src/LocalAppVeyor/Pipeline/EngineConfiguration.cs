using System;
using System.Collections.Generic;
using System.Linq;

namespace LocalAppVeyor.Pipeline
{
    public class EngineConfiguration
    {
        public StepConfiguration[] BuildSteps { get; set; }

        public static EngineConfiguration Default => new EngineConfiguration(new StepConfiguration[0]);

        public EngineConfiguration(IEnumerable<StepConfiguration> buildSteps)
        {
            if (buildSteps == null) throw new ArgumentNullException(nameof(buildSteps));
            
            BuildSteps = buildSteps.ToArray();
        }
    }
}
