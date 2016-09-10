using System;
using System.IO;
using LocalAppVeyor.Configuration.Model;
using LocalAppVeyor.Configuration.Reader.Internal;
using LocalAppVeyor.Configuration.Reader.Internal.Model;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace LocalAppVeyor.Configuration.Reader
{
    public class BuildConfigurationYamlReader : IBuildConfigurationReader
    {
        private const string AppVeyorBuildFileName = "appveyor.yml";

        private string YamlFilePath { get; }

        public BuildConfigurationYamlReader(string yamlFilePathOrDirectory)
        {
            if (string.IsNullOrEmpty(yamlFilePathOrDirectory)) throw new ArgumentNullException(nameof(yamlFilePathOrDirectory));

            string yamlFile;

            if (File.Exists(yamlFilePathOrDirectory))
            {
                YamlFilePath = yamlFilePathOrDirectory;
            }
            else if (Directory.Exists(yamlFilePathOrDirectory) &&
                     File.Exists(yamlFile = Path.Combine(yamlFilePathOrDirectory, AppVeyorBuildFileName)))
            {
                YamlFilePath = yamlFile;
            }
            else
            {
                throw new FileNotFoundException($"'{AppVeyorBuildFileName}' file not found.", yamlFilePathOrDirectory);
            }
        }

        public BuildConfiguration GetBuildConfiguration()
        {
            var yamlDeserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithTypeConverter(new EnvironmentVariablesYamlTypeConverter())
                .WithTypeConverter(new VariableTypeConverter())
                .Build();

            using (var fileStream = new FileStream(YamlFilePath, FileMode.Open))
            using (var streamReader = new StreamReader(fileStream))
            {
                try
                {
                    var conf = yamlDeserializer.Deserialize<InternalBuildConfiguration>(streamReader);

                    return conf?.ToBuildConfiguration() ?? BuildConfiguration.Default;
                }
                catch (YamlException e)
                {
                    throw new LocalAppVeyorException("Error while parsing YAML file.", e);
                }
            }
        }
    }
}