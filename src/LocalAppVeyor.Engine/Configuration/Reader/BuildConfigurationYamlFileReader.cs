using System;
using LocalAppVeyor.Engine.IO;

namespace LocalAppVeyor.Engine.Configuration.Reader
{
    public class BuildConfigurationYamlFileReader : IBuildConfigurationReader
    {
        private const string AppVeyorBuildFileName = "appveyor.yml";

        public string YamlFilePath { get; }

        private readonly FileSystem fileSystem;

        public BuildConfigurationYamlFileReader(string yamlFilePathOrDirectory)
            : this(FileSystem.Default, yamlFilePathOrDirectory)
        {
        }

        public BuildConfigurationYamlFileReader(
            FileSystem fileSystem,
            string yamlFilePathOrDirectory)
        {
            if (string.IsNullOrEmpty(yamlFilePathOrDirectory)) throw new ArgumentNullException(nameof(yamlFilePathOrDirectory));

            this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

            string yamlFile;

            if (fileSystem.File.Exists(yamlFilePathOrDirectory))
            {
                YamlFilePath = yamlFilePathOrDirectory;
            }
            else if (fileSystem.Directory.Exists(yamlFilePathOrDirectory) &&
                     fileSystem.File.Exists(yamlFile = fileSystem.Path.Combine(yamlFilePathOrDirectory, AppVeyorBuildFileName)))
            {
                YamlFilePath = yamlFile;
            }
            else
            {
                throw new LocalAppVeyorException($"'{AppVeyorBuildFileName}' file not found.");
            }
        }

        public BuildConfiguration GetBuildConfiguration()
        {
            string yaml;

            try
            {
                yaml = fileSystem.File.ReadAllText(YamlFilePath);
            }
            catch (Exception e)
            {
                throw new LocalAppVeyorException("Error while reading YAML file.", e);
            }

            return new BuildConfigurationYamlStringReader(yaml).GetBuildConfiguration();
        }
    }
}