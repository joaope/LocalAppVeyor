using System;
using System.IO;
using LocalAppVeyor.Configuration.Model;
using LocalAppVeyor.Configuration.Reader;
using LocalAppVeyor.Pipeline;
using LocalAppVeyor.Pipeline.Output;

namespace LocalAppVeyor.Console
{
    public class Program
    {
        private static readonly IPipelineOutputter PipelineOutputter = new ConsoleOutputter();

        public static void Main(string[] args)
        {
            var engineConfiguration = TryGetEngineConfigurationOrTerminate(@"C:\Users\JoaoP\Desktop\app");
            var buildConfiguration = TryGetBuildConfigurationOrTerminate(engineConfiguration.RepositoryDirectoryPath);

            var engine = new Engine(
                engineConfiguration,
                buildConfiguration);

            engine.Start();

            System.Console.Read();
        }

        private static BuildConfiguration TryGetBuildConfigurationOrTerminate(string repositoryPath)
        {
            var appVeyorYml = Path.Combine(repositoryPath, "appveyor.yml");

            if (!File.Exists(appVeyorYml))
            {
                PipelineOutputter.WriteError($"AppVeyor.yml file not found on repository path. Build aborted.");
                Environment.Exit(1);
            }

            BuildConfiguration configuration = null;

            try
            {
                configuration = new BuildConfigurationYamlReader(appVeyorYml)
                    .GetBuildConfiguration();
            }
            catch (LocalAppVeyorException)
            {
                PipelineOutputter.WriteError($"Error while parsing '{appVeyorYml}' file.");
                PipelineOutputter.WriteError("Build aborted.");
                Environment.Exit(1);
            }

            return configuration;
        }

        private static EngineConfiguration TryGetEngineConfigurationOrTerminate(string repositoryPath)
        {
            // Try infer repository path if one is not provided
            if (!string.IsNullOrEmpty(repositoryPath))
            {
                if (!Directory.Exists(repositoryPath))
                {
                    PipelineOutputter.WriteError($"Repository directory '{repositoryPath}' not found. Build aborted.");
                    Environment.Exit(1);
                }
                else
                {
                    PipelineOutputter.Write($"Using '{repositoryPath}' as the repository path.");
                }
            }
            else
            {
                PipelineOutputter.Write($"Current directory '{repositoryPath}' will be used as repository path.");
                repositoryPath = Directory.GetCurrentDirectory();
            }

            return new EngineConfiguration(repositoryPath, PipelineOutputter);
        }
    }
}
