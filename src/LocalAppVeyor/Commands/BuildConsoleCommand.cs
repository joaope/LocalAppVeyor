using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LocalAppVeyor.Engine;
using LocalAppVeyor.Engine.Configuration;
using LocalAppVeyor.Engine.Configuration.Reader;
using McMaster.Extensions.CommandLineUtils;

namespace LocalAppVeyor.Commands
{
    internal class BuildConsoleCommand : ConsoleCommand
    {
        public override string Name => "build";

        protected override string Description => "Executes appveyor.yml's build jobs from specified repository directory";

        private CommandOption _repositoryPathOption;

        private CommandOption _jobsIndexesOption;

        private CommandOption _skipStepsOptions;

        public BuildConsoleCommand(IPipelineOutputter outputter) 
            : base(outputter)
        {
        }

        protected override void SetUpAdditionalCommandOptions(CommandLineApplication app)
        {
            _repositoryPathOption = app.Option(
                "-d|--dir",
                "Local repository directory where appveyor.yml sits. If not specified current directory is used",
                CommandOptionType.SingleValue);

             _jobsIndexesOption = app.Option(
                "-j|--job",
                "Job to build. You can specify multiple jobs. Use 'jobs' command to list them all",
                CommandOptionType.MultipleValue);

             _skipStepsOptions = app.Option(
                 "-s|--skip",
                 "Step to skip from the build pipeline step. You can specify multiple steps.",
                 CommandOptionType.MultipleValue);
        }

        protected override Task<int> OnExecute(CommandLineApplication app)
        {
            var engineConfiguration = TryGetEngineConfigurationOrTerminate(_repositoryPathOption.Value());
            var buildConfiguration = TryGetBuildConfigurationOrTerminate(engineConfiguration.RepositoryDirectoryPath);

            var engine = new Engine.Engine(
                engineConfiguration,
                buildConfiguration);

            engine.JobStarting += (sender, args) =>
            {
                Outputter.Write($"Starting '{args.Job.Name}'...");
            };

            engine.JobEnded += (sender, args) =>
            {
                switch (args.ExecutionResult.ResultType)
                {
                    case JobExecutionResultType.Success:
                        Outputter.WriteSuccess($"Job '{args.Job.Name}' successfully executed.");
                        break;
                    case JobExecutionResultType.Failure:
                        Outputter.WriteError($"Job '{args.Job.Name}' failed.");
                        break;
                    case JobExecutionResultType.NotExecuted:
                        Outputter.WriteError($"Job '{args.Job.Name}' will not be executed.");
                        break;
                    case JobExecutionResultType.JobNotFound:
                        Outputter.WriteError("Specified job index not found. Use 'jobs' command to list available jobs.");
                        break;
                    case JobExecutionResultType.SolutionFileNotFound:
                        Outputter.WriteError("Solution was not found.");
                        break;
                    case JobExecutionResultType.UnhandledException:
                        Outputter.WriteError($"Unhandled exception while executing '{args.Job.Name}': " +
                                                     $"{args.ExecutionResult.UnhandledException.Message}");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };

            int[] jobs;

            try
            {
                jobs = _jobsIndexesOption
                    .Values
                    .Select(int.Parse)
                    .ToArray();
            }
            catch (Exception)
            {
                Outputter.WriteError("Job option receives a integer as input. Use 'jobs' command to list all available jobs.");
                return Task.FromResult(1);
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
            return Task.FromResult(0);
        }

        private static string ToFinalResultsString(JobExecutionResultType jobResult)
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

        private void PrintFinalResults(IReadOnlyCollection<JobExecutionResult> jobsResults)
        {
            if (jobsResults.Count == 0)
            {
                Outputter.Write("Execution finished.");
                return;
            }

            Outputter.Write("Execution finished:");

            var groupedResults = jobsResults
                .GroupBy(r => r.ResultType)
                .Select(g => $"{ToFinalResultsString(g.Key)}: {g.Count()}");

            Outputter.Write(
                $"=== Execution Result: {string.Join(", ", groupedResults)} ===");
        }

        private BuildConfiguration TryGetBuildConfigurationOrTerminate(string repositoryPathStr)
        {
            var appVeyorYml = Path.Combine(repositoryPathStr, "appveyor.yml");

            if (!File.Exists(appVeyorYml))
            {
                Outputter.WriteError("appveyor.yml file not found on repository path. Trying '.appveyor.yml'...");

                appVeyorYml = Path.Combine(repositoryPathStr, ".appveyor.yml");

                if (!File.Exists(appVeyorYml))
                {
                    Outputter.WriteError(".appveyor.yml file not found on repository path. Build aborted.");
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
                Outputter.WriteError($"Error while parsing '{appVeyorYml}' file. Build aborted.");
                Environment.Exit(1);
            }

            if (_skipStepsOptions.Values != null)
            {
                configuration.SkipSteps = _skipStepsOptions.Values.ToArray();
            }

            return configuration;
        }

        private EngineConfiguration TryGetEngineConfigurationOrTerminate(string repositoryPath)
        {
            if (!string.IsNullOrEmpty(repositoryPath))
            {
                if (!Directory.Exists(repositoryPath))
                {
                    Outputter.WriteError($"Repository directory '{repositoryPath}' not found. Build aborted.");
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
