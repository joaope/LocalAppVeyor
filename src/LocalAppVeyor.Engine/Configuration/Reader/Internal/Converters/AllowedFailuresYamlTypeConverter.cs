using System;
using System.Collections.Generic;
using LocalAppVeyor.Engine.Configuration.Reader.Internal.Model;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace LocalAppVeyor.Engine.Configuration.Reader.Internal.Converters
{
    internal class AllowedFailuresYamlTypeConverter : IYamlTypeConverter
    {
        private readonly IDeserializer _deserializer;

        public AllowedFailuresYamlTypeConverter()
        {
            _deserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithTypeConverter(new VariableTypeConverter())
                .Build();
        }

        public bool Accepts(Type type)
        {
            return type == typeof(AllowedFailuresCollection);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            var allowedFailuresCollection = new AllowedFailuresCollection();

            // discard SequenceStart
            parser.Expect<SequenceStart>();

            do
            {
                string os = null;
                string configuration = null;
                string platform = null;
                string testCategory = null;
                var variables = new List<Variable>();

                parser.Expect<MappingStart>();

                do
                {
                    var possibleVar = _deserializer.Deserialize<InternalVariable>(parser);

                    switch (possibleVar.Name)
                    {
                        case "os":
                            os = possibleVar.Value;
                            break;
                        case "configuration":
                            configuration = possibleVar.Value;
                            break;
                        case "platform":
                            platform = possibleVar.Value;
                            break;
                        case "test_category":
                            testCategory = possibleVar.Value;
                            break;
                        default:
                            variables.Add(possibleVar.ToVariable());
                            break;
                    }
                } while (!parser.Accept<MappingEnd>());

                parser.Expect<MappingEnd>();

                allowedFailuresCollection.Add(
                    new AllowedJobFailureConditions(os, configuration, platform, testCategory, variables.AsReadOnly()));

            } while (!parser.Accept<SequenceEnd>());

            parser.Expect<SequenceEnd>();

            return allowedFailuresCollection;
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            throw new NotImplementedException();
        }
    }
}