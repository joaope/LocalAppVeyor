using System;
using System.IO;
using LocalAppVeyor.Configuration.Model;
using LocalAppVeyor.Configuration.Reader.Converters;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

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
            var yamlDeserializer = new Deserializer(namingConvention: new CamelCaseNamingConvention(), ignoreUnmatched: true);

            yamlDeserializer.RegisterTypeConverter(new EnvironmentVariablesYamlTypeConverter(yamlDeserializer));
            yamlDeserializer.RegisterTypeConverter(new VariableTypeConverter());

            using (var fileStream = new FileStream(YamlFilePath, FileMode.Open))
            using (var streamReader = new StreamReader(fileStream))
            {
                try
                {
                    var conf = yamlDeserializer.Deserialize<BuildConfiguration>(streamReader);
                    return conf;
                }
                catch (YamlException e)
                {
                    if (e.Start != null && e.End != null)
                    {
                        throw new LocalAppVeyorYamlParsingException(
                            "Error while parsing YAML.",
                            e.Start.Line, e.Start.Column, e.End.Line, e.End.Column);
                    }

                    throw new LocalAppVeyorYamlParsingException("Unknown error while parsing YAML.");
                }
            }
        }
    }
}