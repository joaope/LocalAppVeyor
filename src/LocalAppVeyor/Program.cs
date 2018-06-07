using System;
using System.Reflection;
using LocalAppVeyor.Commands;
using LocalAppVeyor.Engine;
using McMaster.Extensions.CommandLineUtils;

namespace LocalAppVeyor
{
    public static class Program
    {
        private static readonly IPipelineOutputter PipelineOutputter = new ConsoleOutputter();

        private static readonly BuildConsoleCommand BuildCommand = new BuildConsoleCommand(PipelineOutputter);

        private static readonly JobsConsoleCommand JobsCommand = new JobsConsoleCommand(PipelineOutputter);

        private static readonly LintConsoleCommand LintCommand = new LintConsoleCommand(PipelineOutputter);

        public static void Main(string[] args)
        {
            var app = new CommandLineApplication(false)
            {
                Name = "LocalAppVeyor",
                FullName = "LocalAppVeyor",
                Description = "LocalAppVeyor allows one to run an appveyor.yml build script locally"
            };

            var (shortFormVersion, longFormVersion) = GetShortAndLongVersion();

            app.HelpOption("-?|-h|--help");
            app.VersionOption("-v|--version", shortFormVersion, longFormVersion);

            app.Command(BuildCommand.Name, conf => { BuildCommand.SetUp(conf); }, false);
            app.Command(JobsCommand.Name, conf => { JobsCommand.SetUp(conf); }, false);
            app.Command(LintCommand.Name, conf => { LintCommand.SetUp(conf); }, false);

            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 0;
            });

            app.Execute(args);
        }

        private static (string ShortFormVersion, string LongFormVersion) GetShortAndLongVersion()
        {
            string GetVersionFromTypeInfo(Type typeInfo)
            {
                var infoVersion = typeInfo.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion;

                if (string.IsNullOrEmpty(infoVersion))
                {
                    infoVersion = typeInfo.Assembly.GetName().Version.ToString();
                }

                return infoVersion;
            }

            var consoleVer = GetVersionFromTypeInfo(typeof(Program).GetTypeInfo());
            var engineVer = GetVersionFromTypeInfo(typeof(Engine.Engine).GetTypeInfo());

            return (consoleVer, $"{consoleVer} (engine: {engineVer})");
        }
    }
}
