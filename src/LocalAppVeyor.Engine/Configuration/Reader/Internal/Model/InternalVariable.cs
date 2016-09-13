namespace LocalAppVeyor.Engine.Configuration.Reader.Internal.Model
{
    internal class InternalVariable
    {
        public string Name { get; }    

        public bool IsSecuredValue { get; }

        public string Value { get;  }

        public InternalVariable(string name, string value, bool isSecuredValue)
        {
            Name = name;
            Value = value;
            IsSecuredValue = isSecuredValue;
        }

        public Variable ToVariable()
        {
            return new Variable(Name, Value, IsSecuredValue);
        }
    }
}
