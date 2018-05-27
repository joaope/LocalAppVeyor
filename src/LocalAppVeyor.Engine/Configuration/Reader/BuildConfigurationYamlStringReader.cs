using LocalAppVeyor.Engine.Configuration.Reader.Internal.Converters;
using LocalAppVeyor.Engine.Configuration.Reader.Internal.Model;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace LocalAppVeyor.Engine.Configuration.Reader
{
    public class BuildConfigurationYamlStringReader : IBuildConfigurationReader
    {
        public string Yaml { get; }

        public BuildConfigurationYamlStringReader(string yaml)
        {
            Yaml = yaml;
        }

        public BuildConfiguration GetBuildConfiguration()
        {
            if (string.IsNullOrEmpty(Yaml))
            {
                return BuildConfiguration.Default;
            }

            var yamlDeserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithTypeConverter(new EnvironmentVariablesYamlTypeConverter())
                .WithTypeConverter(new VariableTypeConverter())
                .WithTypeConverter(new AllowedFailuresYamlTypeConverter())
                .Build();

            try
            {
                var conf = yamlDeserializer.Deserialize<InternalBuildConfiguration>(Yaml);

                return conf?.ToBuildConfiguration() ?? BuildConfiguration.Default;
            }
            catch (YamlException e)
            {
                throw new LocalAppVeyorException("Error while parsing YAML.", e);
            }
        }
    }
}