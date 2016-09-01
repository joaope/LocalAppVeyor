using System;

namespace LocalAppVeyor.Console
{
    internal class StepConfiguration
    {
        public string Name { get; set; }

        public bool ContinueOnFail { get; set; }

        public string Before { get; set; }

        public string After { get; set; }

        private string typeName;

        public string TypeName
        {
            get
            {
                return typeName;
            }
            set
            {
                typeName = value;
                Type = Type.GetType(value);
            }
        }

        public Type Type { get; private set; }
    }
}