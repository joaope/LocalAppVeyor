using System;

namespace LocalAppVeyor.Pipeline
{
    public class StepConfiguration
    {
        public string Name { get; set; }

        public bool ContinueOnFail { get; }

        public Type StepType { get; }
        
        public StepConfiguration(string name, Type stepType)
            : this(name, stepType, false)
        {
        }

        public StepConfiguration(
            string name,
            Type stepType,
            bool continueOnFail)
        {
            if (stepType == null) throw new ArgumentNullException(nameof(stepType));

            Name = name;
            ContinueOnFail = continueOnFail;
            StepType = stepType;
        }
    }
}