using System.Diagnostics;

namespace LocalAppVeyor.Configuration.Model
{
    [DebuggerDisplay("Name = {Name}, Value = {Value}, IsSecure = {IsSecuredValue}")]
    public class Variable
    {
        public string Name { get; }    

        public bool IsSecuredValue { get; }

        public string Value { get;  }

        public Variable(string name, string value, bool isSecuredValue)
        {
            Name = name;
            Value = value;
            IsSecuredValue = isSecuredValue;
        }
    }
}
