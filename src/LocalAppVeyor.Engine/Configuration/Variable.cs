using System;

namespace LocalAppVeyor.Engine.Configuration
{
    public class Variable : IEquatable<Variable>
    {
        public string Name { get; }    

        public bool IsSecuredValue { get; }

        public ExpandableString Value { get; }

        public Variable(string name, string value, bool isSecuredValue)
        {
            Name = name;
            Value = value;
            IsSecuredValue = isSecuredValue;
        }

        public bool Equals(Variable other)
        {
            if (other == null)
            {
                return false;
            }

            return Name == other.Name &&
                   Value == other.Value &&
                   IsSecuredValue == other.IsSecuredValue;
        }

        public override string ToString()
        {
            return $"{Name}={Value}";
        }
    }
}
