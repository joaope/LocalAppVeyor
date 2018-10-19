using System;
using System.Collections.Generic;
using LocalAppVeyor.Engine.Configuration.Reader.Internal.Model;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace LocalAppVeyor.Engine.Configuration.Reader.Internal.Converters
{
    internal class EnvironmentVariablesYamlTypeConverter : IYamlTypeConverter
    {
        private readonly IDeserializer _deserializer;

        public EnvironmentVariablesYamlTypeConverter()
        {
            _deserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithTypeConverter(new VariableTypeConverter())
                .Build();
        }

        public bool Accepts(Type type)
        {
            return type == typeof(InternalEnvironmentVariables);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            var env = new InternalEnvironmentVariables();

            parser.Expect<MappingStart>();

            do
            {
                var scalar = parser.Peek<Scalar>();

                if (scalar != null)
                {
                    if (scalar.Value == "global")
                    {
                        // discard "global" value itself
                        parser.Expect<Scalar>();

                        // read global variables (common to all matrix items)
                        parser.Expect<MappingStart>();

                        do
                        {
                            env.InternalCommonVariables.Add(_deserializer.Deserialize<InternalVariable>(parser));

                        } while (!parser.Accept<MappingEnd>());

                        parser.Expect<MappingEnd>();

                    }
                    else if (scalar.Value == "matrix")
                    {
                        // discard "matrix" value itself
                        parser.Expect<Scalar>();

                        // discard SequenceStart
                        parser.Expect<SequenceStart>();

                        do
                        {
                            var matrixItemVariables = new List<InternalVariable>();

                            parser.Expect<MappingStart>();

                            do
                            {
                                matrixItemVariables.Add(_deserializer.Deserialize<InternalVariable>(parser));

                            } while (!parser.Accept<MappingEnd>());

                            parser.Expect<MappingEnd>();

                            env.InternalMatrix.Add(matrixItemVariables.AsReadOnly());

                        } while (!parser.Accept<SequenceEnd>());

                        parser.Expect<SequenceEnd>();
                    }
                    else
                    {
                        var variable = _deserializer.Deserialize<InternalVariable>(parser);
                        env.InternalCommonVariables.Add(variable);
                    }
                }
                
                
            } while (!parser.Accept<MappingEnd>());

            parser.Expect<MappingEnd>();

            return env;
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            throw new NotImplementedException();
        }
    }
}