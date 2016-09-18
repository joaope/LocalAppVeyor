namespace LocalAppVeyor.Engine.Configuration
{
    public class Variable
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

        public override string ToString()
        {
            return $"{Name}={Value}";
        }
    }
}
