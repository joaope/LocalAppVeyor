using System;
using System.Collections.Generic;
using LocalAppVeyor.Configuration.Model;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace LocalAppVeyor.Configuration.Reader.Converters
{
    internal class EnvironmentVariablesYamlTypeConverter : IYamlTypeConverter
    {
        private readonly Deserializer deserializer;

        public EnvironmentVariablesYamlTypeConverter(Deserializer deserializer)
        {
            this.deserializer = deserializer;
        }

        public bool Accepts(Type type)
        {
            return type == typeof(EnvironmentVariables);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            var env = new EnvironmentVariables();

            parser.Expect<MappingStart>();

            do
            {
                var scalar = parser.Peek<Scalar>();

                if (scalar != null)
                {
                    if (scalar.Value == "matrix")
                    {
                        // discard "matrix" value itself
                        parser.Expect<Scalar>();

                        // discard SequenceStart
                        parser.Expect<SequenceStart>();

                        do
                        {
                            var matrixItemVariables = new List<Variable>();

                            parser.Expect<MappingStart>();

                            do
                            {
                                matrixItemVariables.Add(deserializer.Deserialize<Variable>(parser));

                            } while (!parser.Accept<MappingEnd>());

                            parser.Expect<MappingEnd>();

                            env.InternalMatrix.Add(matrixItemVariables.AsReadOnly());

                        } while (!parser.Accept<SequenceEnd>());

                        parser.Expect<SequenceEnd>();
                    }
                    else
                    {
                        var variable = deserializer.Deserialize<Variable>(parser);
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