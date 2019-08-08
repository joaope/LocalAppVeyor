using System;
using System.IO;
using System.Threading.Tasks;
using LocalAppVeyor.Engine;
using LocalAppVeyor.Engine.Configuration;
using LocalAppVeyor.Engine.Configuration.Reader;
using McMaster.Extensions.CommandLineUtils;

namespace LocalAppVeyor.Commands
{
    internal class JobsConsoleCommand : ConsoleCommand
    {
        private CommandOption _repositoryPathOption;

        public override string Name => "jobs";

        protected override string Description => "List all build jobs available to execution";

        public JobsConsoleCommand(IPipelineOutputter outputter) 
            : base(outputter)
        {
        }

        protected override void SetUpAdditionalCommandOptions(CommandLineApplication app)
        {
            _repositoryPathOption = app.Option(
                "-d|--dir",
                "Local repository directory where appveyor.yml sits. If not specified current directory is used",
                CommandOptionType.SingleValue);
        }

        protected override Task<int> OnExecute(CommandLineApplication app)
        {
            var engineConfiguration = TryGetEngineConfigurationOrTerminate(_repositoryPathOption.Value());
            var buildConfiguration = TryGetBuildConfigurationOrTerminate(engineConfiguration.RepositoryDirectoryPath);

            var engine = new Engine.Engine(
                engineConfiguration,
                buildConfiguration);

            engineConfiguration.Outputter.Write("Available jobs:");
            for (var i = 0; i < engine.Jobs.Length; i++)
            {
                engineConfiguration.Outputter.Write(
                    $"[{i}]: {engine.Jobs[i].Name}");
            }

            return Task.FromResult(0);
        }

        private BuildConfiguration TryGetBuildConfigurationOrTerminate(string repositoryPathStr)
        {
            var appVeyorYml = Path.Combine(repositoryPathStr, "appveyor.yml");

            if (!File.Exists(appVeyorYml))
            {
                Outputter.Write("appveyor.yml file not found on repository path. Trying '.appveyor.yml'...");

                appVeyorYml = Path.Combine(repositoryPathStr, ".appveyor.yml");

                if (!File.Exists(appVeyorYml))
                {
                    Outputter.WriteError(".appveyor.yml file not found on repository path.");
                    Environment.Exit(1);
                }
            }

            BuildConfiguration configuration = null;

            try
            {
                configuration = new BuildConfigurationYamlFileReader(appVeyorYml)
                    .GetBuildConfiguration();
            }
            catch (LocalAppVeyorException)
            {
                Outputter.WriteError($"Error while parsing '{appVeyorYml}' file.");
                Environment.Exit(1);
            }

            return configuration;
        }

        private EngineConfiguration TryGetEngineConfigurationOrTerminate(string repositoryPath)
        {
            if (!string.IsNullOrEmpty(repositoryPath))
            {
                if (!Directory.Exists(repositoryPath))
                {
                    Outputter.WriteError($"Repository directory '{repositoryPath}' not found.");
                    Environment.Exit(1);
                }
            }
            else
            {
                repositoryPath = Directory.GetCurrentDirectory();
            }

            return new EngineConfiguration(repositoryPath, Outputter);
        }
    }
}