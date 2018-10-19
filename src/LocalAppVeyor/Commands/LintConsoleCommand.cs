using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Threading.Tasks;
using LocalAppVeyor.Engine;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace LocalAppVeyor.Commands
{
    internal sealed class LintConsoleCommand : ConsoleCommand
    {
        private const string TokenEnvironmentVariableName = "LOCALAPPVEYOR_API_TOKEN";

        private const string YmlValidationUrl = "https://ci.appveyor.com/api/projects/validate-yaml";

        private CommandOption _repositoryPathOption;

        private CommandOption _apiTokenOption;

        public override string Name => "lint";

        protected override string Description => "Validates appveyor.yml YAML configuration. It requires internet connection.";

        public LintConsoleCommand(IPipelineOutputter outputter)
            : base(outputter)
        {
        }

        protected override void SetUpAdditionalCommandOptions(CommandLineApplication app)
        {
            _apiTokenOption = app.Option(
                "-t|--token",
                $"Your AppVeyor account API token. If not specified it tries to get it from {TokenEnvironmentVariableName} " +
                "environment variable. You can find your API token here: https://ci.appveyor.com/api-token.",
                CommandOptionType.SingleValue);

            _repositoryPathOption = app.Option(
                "-d|--dir",
                "Local repository directory where appveyor.yml sits. If not specified current directory is used",
                CommandOptionType.SingleValue);
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            var apiToken = _apiTokenOption.Value();

            if (string.IsNullOrEmpty(apiToken))
            {
                apiToken = Environment.GetEnvironmentVariable(
                    TokenEnvironmentVariableName,
                    EnvironmentVariableTarget.User);

                if (string.IsNullOrEmpty(apiToken))
                {
                    Outputter.WriteError("AppVeyor API token is required. Either specify it using  --token " +
                                         $"option or {TokenEnvironmentVariableName} environment variable.");
                    Environment.Exit(1);
                }
            }

            var repositoryPath = TryGetRepositoryDirectoryPathOrTerminate(_repositoryPathOption.Value());
            var (yamlFilePath, ymlFileContent) = TryGetAppVeyorFileContentOrTerminate(repositoryPath);

            Outputter.Write($"Validating '{yamlFilePath}'...");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);

                Outputter.Write("Connecting to AppVeyor validation API...");

                try
                {
                    using (var response = await client.PostAsync(YmlValidationUrl, new StringContent(ymlFileContent)))
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseObj = JsonConvert.DeserializeObject<dynamic>(responseContent);

                        switch (response.StatusCode)
                        {
                            case HttpStatusCode.OK when (bool)responseObj.isValid:
                                Outputter.WriteSuccess("YAML configuration file is valid.");
                                return 0;
                            case HttpStatusCode.OK:
                                Outputter.WriteError((string)responseObj.errorMessage);
                                break;
                            case HttpStatusCode.Unauthorized:
                                Outputter.WriteError("Authorization failed. Make sure you're specifying an updated API token.");
                                break;
                            default:
                                var msg = (string) responseObj.message;
                                Outputter.WriteError(!string.IsNullOrEmpty(msg)
                                    ? $"Validation failed with status code {response.StatusCode}. Message: {msg}"
                                    : $"Validation failed with status code {response.StatusCode}.");

                                break;
                        }
                    }
                }
                catch (HttpRequestException)
                {
                    Outputter.WriteError("Error connecting to AppVeyor validation API. Check your interner connection.");
                }
            }

            return 1;
        }

        private (string YamlFilePath, string YmlFileContent) TryGetAppVeyorFileContentOrTerminate(string repositoryPath)
        {
            var appVeyorYml = Path.Combine(repositoryPath, "appveyor.yml");

            if (!File.Exists(appVeyorYml))
            {
                Outputter.Write("appveyor.yml file not found on repository path. Trying '.appveyor.yml'...");

                appVeyorYml = Path.Combine(repositoryPath, ".appveyor.yml");

                if (!File.Exists(appVeyorYml))
                {
                    Outputter.WriteError(".appveyor.yml file not found on repository path. Validation stopped.");
                    Environment.Exit(1);
                }
            }

            string exceptionReason;

            try
            {
                return (appVeyorYml, File.ReadAllText(appVeyorYml));
            }
            catch (PathTooLongException)
            {
                exceptionReason = "Path too long";
            }
            catch (DirectoryNotFoundException)
            {
                exceptionReason = "Directory not found";
            }
            catch (FileNotFoundException)
            {
                exceptionReason = "File not found";
            }
            catch (NotSupportedException)
            {
                exceptionReason = "Path is in an invalid format";
            }
            catch (IOException e)
            {
                exceptionReason = e.Message;
            }
            catch (UnauthorizedAccessException)
            {
                exceptionReason = "No permissions to read configuration file";
            }
            catch (SecurityException)
            {
                exceptionReason = "The caller does not have the required permission";
            }

            Outputter.WriteError($"Error while trying to read '{appVeyorYml}' file. {exceptionReason}. Validation aborted.");
            Environment.Exit(1);
            return (null, null);
        }

        private string TryGetRepositoryDirectoryPathOrTerminate(string repositoryPath)
        {
            if (!string.IsNullOrEmpty(repositoryPath))
            {
                if (!Directory.Exists(repositoryPath))
                {
                    Outputter.WriteError($"Repository directory '{repositoryPath}' not found. Validation aborted.");
                    Environment.Exit(1);
                }

                return repositoryPath;
            }

            return Directory.GetCurrentDirectory();
        }
    }
}