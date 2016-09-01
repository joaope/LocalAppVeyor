using System;
using System.IO;
using LocalAppVeyor.Configuration.Model;
using LocalAppVeyor.Configuration.Readers.Converters;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LocalAppVeyor.Configuration.Readers
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
            var yamlDeserializer = new Deserializer(namingConvention: new CamelCaseNamingConvention(), ignoreUnmatched: true);

            yamlDeserializer.RegisterTypeConverter(new EnvironmentVariablesYamlTypeConverter(yamlDeserializer));
            yamlDeserializer.RegisterTypeConverter(new VariableTypeConverter(yamlDeserializer));

            using (var fileStream = new FileStream(YamlFilePath, FileMode.Open))
            using (var streamReader = new StreamReader(fileStream))
            {
                var conf = yamlDeserializer.Deserialize<BuildConfiguration>(streamReader);
                return conf;
            }
        }
    }
}