using System.Diagnostics;

namespace LocalAppVeyor.Configuration.Model
{
    [DebuggerDisplay("Name = {Name}, Value = {Value}, IsSecure = {IsSecured}")]
    public class Variable
    {
        public string Name { get; }    

        public bool IsSecured { get; }

        public string Value { get;  }

        public Variable(string name, string value)
            : this(name, value, false)
        {
        }
        
        public Variable(string name, string value, bool isSecured)
        {
            Name = name;
            Value = value;
            IsSecured = isSecured;
        }
    }
}
