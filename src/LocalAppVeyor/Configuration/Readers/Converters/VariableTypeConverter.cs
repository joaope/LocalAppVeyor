using System;
using LocalAppVeyor.Configuration.Model;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace LocalAppVeyor.Configuration.Readers.Converters
{
    public class VariableTypeConverter : IYamlTypeConverter
    {
        private readonly Deserializer deserializer;

        public VariableTypeConverter(Deserializer deserializer)
        {
            this.deserializer = deserializer;
        }

        public bool Accepts(Type type)
        {
            return type == typeof(Variable);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            var name = parser.Expect<Scalar>().Value;

            var mappingStart = parser.Allow<MappingStart>();

            if (mappingStart != null)
            {
                var secureNode = parser.Expect<Scalar>();

                if (secureNode != null && secureNode.Value == "secure")
                {
                    var secureValue = parser.Expect<Scalar>().Value;

                    // for sure we'll have a MappingEnd, otherwise let throw
                    parser.Expect<MappingEnd>();

                    return new Variable(name, secureValue, true);
                }

                throw new YamlException("error parsing enrivonment variables");
            }
            
            return new Variable(name, parser.Expect<Scalar>().Value, false);
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            throw new NotImplementedException();
        }
    }
}