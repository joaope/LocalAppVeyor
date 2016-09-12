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
                conf.Description = "Executes one or all build jobs on specified repository directory";

                var repositoryPath = conf.Option(
                    "--dir",
                    "Repository directory where appveyor.yml is. If not specified, current directory is used",
                    CommandOptionType.SingleValue);

                var jobsIndexes = conf.Option(
                    "--job",
                    "Executes specified build jobs. Use 'jobs' command to list all available jobs.",
                    CommandOptionType.MultipleValue);

                conf.OnExecute(() =>
                {
                    conf.ShowRootCommandFullNameAndVersion();

                    var engineConfiguration = TryGetEngineConfigurationOrTerminate(repositoryPath.Value());
                    var buildConfiguration = TryGetBuildConfigurationOrTerminate(engineConfiguration.RepositoryDirectoryPath);

                    var engine = new Engine.Pipeline.Engine(
                        engineConfiguration,
                        buildConfiguration);

                    engine.ExecuteAllJobs(); // TODO: do something with jobs results
                    return 0;
                });
            }, false);

            app.Command("jobs", conf =>
            {
                conf.Description = "List all build jobs available to execution.";

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

                    engineConfiguration.Outputter.Write("Available jobs:");
                    for (var i = 0; i < engine.Jobs.Length; i++)
                    {
                        engineConfiguration.Outputter.Write(
                            $"[{i}]: {engine.Jobs[i].Name}");
                    }

                    return 0;
                });

            }, false);

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
                PipelineOutputter.WriteError($"Error while parsing '{appVeyorYml}' file. Build aborted.");
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
            }
            else
            {
                repositoryPath = Directory.GetCurrentDirectory();
            }

            return new EngineConfiguration(repositoryPath, PipelineOutputter);
        }
        
    }
}
