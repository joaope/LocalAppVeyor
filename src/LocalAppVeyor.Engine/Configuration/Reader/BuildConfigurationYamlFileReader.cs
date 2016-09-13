using System;
using System.IO;
using LocalAppVeyor.Engine.Configuration.Model;

namespace LocalAppVeyor.Engine.Configuration.Reader
{
    public class BuildConfigurationYamlFileReader : IBuildConfigurationReader
    {
        private const string AppVeyorBuildFileName = "appveyor.yml";

        public string YamlFilePath { get; }

        public BuildConfigurationYamlFileReader(string yamlFilePathOrDirectory)
        {
            if (string.IsNullOrEmpty(yamlFilePathOrDirectory))
                throw new ArgumentNullException(nameof(yamlFilePathOrDirectory));

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
            string yaml;

            try
            {
                yaml = File.ReadAllText(YamlFilePath);
            }
            catch (Exception e)
            {
                throw new LocalAppVeyorException("Error while reading YAML file.", e);
            }

            return new BuildConfigurationYamlStringReader(yaml).GetBuildConfiguration();
        }
    }
}