using System;
using System.IO;
using LocalAppVeyor.Engine;
using LocalAppVeyor.Engine.Configuration.Model;
using LocalAppVeyor.Engine.Configuration.Reader;
using LocalAppVeyor.Engine.Pipeline;
using Microsoft.Extensions.CommandLineUtils;

namespace LocalAppVeyor
{
    public class Program
    {
        private static readonly IPipelineOutputter PipelineOutputter = new ConsoleOutputter();

        public static void Main(string[] args)
        {
            var app = new CommandLineApplication(false)
            {
                Name = "LocalAppVeyor",
                FullName = "LocalAppVeyor",
                Description = "LocalAppVeyor allows one to run an appveyor.yml build script locally."
            };

            app.HelpOption("-?|-h|--help");
            app.VersionOption("-v|--version", "0.5.0");

            app.Command("build", conf =>
            {
                conf.Description = "Executes appveyor.yml build script";

                var repositoryPath = conf.Option(
                    "--dir",
                    "Repository directory where appveyor.yml is. If not specified, current directory is used",
                    CommandOptionType.SingleValue);

                conf.OnExecute(() =>
                {
                    conf.ShowRootCommandFullNameAndVersion();

                    var engineConfiguration = TryGetEngineConfigurationOrTerminate(repositoryPath.Value());
                    var buildConfiguration = TryGetBuildConfigurationOrTerminate(engineConfiguration.RepositoryDirectoryPath);

                    var engine = new Engine.Pipeline.Engine(
                        engineConfiguration,
                        buildConfiguration);

                    return engine.Start() ? 0 : 1;
                });
            });

            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 0;
            });

            app.Execute(args);
        }

        private static BuildConfiguration TryGetBuildConfigurationOrTerminate(string repositoryPath)
        {
            var appVeyorYml = Path.Combine(repositoryPath, "appveyor.yml");

            if (!File.Exists(appVeyorYml))
            {
                PipelineOutputter.WriteError("AppVeyor.yml file not found on repository path. Build aborted.");
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
                repositoryPath = Directory.GetCurrentDirectory();
                PipelineOutputter.Write($"Current directory '{repositoryPath}' will be used as repository path.");
            }

            return new EngineConfiguration(repositoryPath, PipelineOutputter);
        }
        
    }
}
