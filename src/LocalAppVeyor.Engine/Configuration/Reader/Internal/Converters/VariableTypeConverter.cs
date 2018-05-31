using System;
using LocalAppVeyor.Engine.Configuration.Reader.Internal.Model;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace LocalAppVeyor.Engine.Configuration.Reader.Internal.Converters
{
    internal class VariableTypeConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(InternalVariable);
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

                    return new InternalVariable(name, secureValue, true);
                }

                throw new YamlException("error parsing environment variables");
            }
            
            return new InternalVariable(name, parser.Expect<Scalar>().Value, false);
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            throw new NotImplementedException();
        }
    }
}