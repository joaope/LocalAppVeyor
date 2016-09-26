using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LocalAppVeyor.Engine;
using LocalAppVeyor.Engine.Configuration;
using LocalAppVeyor.Engine.Configuration.Reader;
using LocalAppVeyor.Engine.IO;
using Microsoft.Extensions.CommandLineUtils;

namespace LocalAppVeyor
{
    public static class Program
    {
        private static readonly IPipelineOutputter PipelineOutputter = new ConsoleOutputter();

        public static void Main(string[] args)
        {
            var app = new CommandLineApplication(false)
            {
                Name = "LocalAppVeyor",
                FullName = "LocalAppVeyor",
                Description = "LocalAppVeyor allows one to run an appveyor.yml build script locally"
            };

            var versions = GetShortAndLongVersion();

            app.HelpOption("-?|-h|--help");
            app.VersionOption("-v|--version", versions.Item1, versions.Item2);

            app.Command("build", conf =>
            {
                conf.Description = "Executes appveyor.yml's build jobs from specified repository directory";
                conf.HelpOption("-?|-h|--help");

                var repositoryPath = conf.Option(
                    "-d|--dir",
                    "Local repository directory where appveyor.yml sits. If not specified current directory is used",
                    CommandOptionType.SingleValue);

                var jobsIndexes = conf.Option(
                    "-j|--job",
                    "Job to build. You can specify multiple jobs. Use 'jobs' command to list them all",
                    CommandOptionType.MultipleValue);

                conf.OnExecute(() => ExecuteBuildCommand(conf, repositoryPath, jobsIndexes));
            }, false);

            app.Command("jobs", conf =>
            {
                conf.Description = "List all build jobs available to execution";
                conf.HelpOption("-?|-h|--help");

                var repositoryPath = conf.Option(
                    "-d|--dir",
                    "Local repository directory where appveyor.yml sits. If not specified current directory is used",
                    CommandOptionType.SingleValue);

                conf.OnExecute(() =>
                {
                    conf.ShowRootCommandFullNameAndVersion();

                    var engineConfiguration = TryGetEngineConfigurationOrTerminate(repositoryPath.Value());
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

        private static Tuple<string, string> GetShortAndLongVersion()
        {
            Func<TypeInfo, string> getVersionFromTypeInfo = typeInfo =>
            {
                var infoVersion =
                    typeInfo
                    .Assembly
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion;

                if (string.IsNullOrEmpty(infoVersion))
                {
                    infoVersion = typeInfo.Assembly.GetName().Version.ToString();
                }

                return infoVersion;
            };

            var consoleVer = getVersionFromTypeInfo(typeof(Program).GetTypeInfo());
            var engineVer = getVersionFromTypeInfo(typeof(Engine.Engine).GetTypeInfo());

            return new Tuple<string, string>(
                consoleVer,
                $"{consoleVer} (engine: {engineVer})");
        }

        private static BuildConfiguration TryGetBuildConfigurationOrTerminate(string repositoryPath)
        {
            var appVeyorYml = Path.Combine(repositoryPath, "appveyor.yml");

            if (!File.Exists(appVeyorYml))
            {
                PipelineOutputter.WriteError("appveyor.yml file not found on repository path. Trying '.appveyor.yml'...");

                appVeyorYml = Path.Combine(repositoryPath, ".appveyor.yml");

                if (!File.Exists(appVeyorYml))
                {
                    PipelineOutputter.WriteError(".appveyor.yml file not found on repository path. Build aborted.");
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
                PipelineOutputter.WriteError($"Error while parsing '{appVeyorYml}' file. Build aborted.");
                Environment.Exit(1);
            }

            return configuration;
        }

        private static EngineConfiguration TryGetEngineConfigurationOrTerminate(string repositoryPath)
        {
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

            return new EngineConfiguration(repositoryPath, PipelineOutputter, FileSystem.Default);
        }

        private static int ExecuteBuildCommand(
            CommandLineApplication app,
            CommandOption repositoryPathOption,
            CommandOption jobsOption)
        {
            app.ShowRootCommandFullNameAndVersion();

            var engineConfiguration = TryGetEngineConfigurationOrTerminate(repositoryPathOption.Value());
            var buildConfiguration =
                TryGetBuildConfigurationOrTerminate(engineConfiguration.RepositoryDirectoryPath);

            var engine = new Engine.Engine(
                engineConfiguration,
                buildConfiguration);

            engine.JobStarting += (sender, args) =>
            {
                PipelineOutputter.Write($"Starting '{args.Job.Name}'...");
            };

            engine.JobEnded += (sender, args) =>
            {
                switch (args.ExecutionResult.ResultType)
                {
                    case JobExecutionResultType.Success:
                        PipelineOutputter.WriteSuccess($"Job '{args.Job.Name}' successfully executed.");
                        break;
                    case JobExecutionResultType.Failure:
                        PipelineOutputter.WriteError($"Job '{args.Job.Name}' failed.");
                        break;
                    case JobExecutionResultType.NotExecuted:
                        PipelineOutputter.WriteError($"Job '{args.Job.Name}' will not be executed.");
                        break;
                    case JobExecutionResultType.JobNotFound:
                        PipelineOutputter.WriteError("Specified job index not found. Use 'jobs' command to list available jobs.");
                        break;
                    case JobExecutionResultType.SolutionFileNotFound:
                        PipelineOutputter.WriteError("Solution was not found.");
                        break;
                    case JobExecutionResultType.UnhandledException:
                        PipelineOutputter.WriteError($"Unhandled exception while executing '{args.Job.Name}': " +
                                                     $"{args.ExecutionResult.UnhandledException.Message}");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };

            int[] jobs;

            try
            {
                jobs = jobsOption
                    .Values
                    .Select(int.Parse)
                    .ToArray();
            }
            catch (Exception)
            {
                PipelineOutputter.WriteError("Job option receives a integer as input. Use 'jobs' command to list all available jobs.");
                return 1;
            }

            var jobsResults = new List<JobExecutionResult>();

            if (jobs.Length == 0)
            {
                jobsResults = engine.ExecuteAllJobs().ToList();
            }
            else
            {
                foreach (var jobIndex in jobs)
                {
                    jobsResults.Add(engine.ExecuteJob(jobIndex));
                }
            }

            PrintFinalResults(jobsResults);
            return 0;
        }

        private static string ToFinalResultsString(this JobExecutionResultType jobResult)
        {
            switch (jobResult)
            {
                case JobExecutionResultType.Success:
                    return "Succeeded";
                case JobExecutionResultType.Failure:
                    return "Failed";
                case JobExecutionResultType.NotExecuted:
                    return "Not executed";
                case JobExecutionResultType.JobNotFound:
                    return "Job not found";
                case JobExecutionResultType.SolutionFileNotFound:
                    return "No solution file";
                case JobExecutionResultType.UnhandledException:
                    return "Unhandled exception";
                default:
                    throw new ArgumentOutOfRangeException(nameof(jobResult), jobResult, null);
            }
        }

        private static void PrintFinalResults(List<JobExecutionResult> jobsResults)
        {
            if (jobsResults.Count == 0)
            {
                PipelineOutputter.Write("Execution finished.");
                return;
            }

            PipelineOutputter.Write("Execution finished:");

            var groupedResults = jobsResults
                .GroupBy(r => r.ResultType)
                .Select(g => $"{g.Key.ToFinalResultsString()}: {g.Count()}");

            PipelineOutputter.Write(
                $"=== Execution Result: {string.Join(", ", groupedResults)} ===");
        }
    }
}
