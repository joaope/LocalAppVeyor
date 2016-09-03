using System;
using LocalAppVeyor.Configuration.Model;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace LocalAppVeyor.Configuration.Reader.Converters
{
    internal class VariableTypeConverter : IYamlTypeConverter
    {
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