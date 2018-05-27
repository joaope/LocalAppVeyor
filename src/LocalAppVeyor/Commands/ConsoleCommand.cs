using System.Collections.Generic;
using LocalAppVeyor.Engine;
using Microsoft.Extensions.CommandLineUtils;

namespace LocalAppVeyor.Commands
{
    internal abstract class ConsoleCommand
    {
        public abstract string Name { get; }

        protected abstract string Description { get; }

        protected IPipelineOutputter Outputter { get; }

        protected ConsoleCommand(IPipelineOutputter outputter)
        {
            Outputter = outputter;
        }

        public void SetUp(CommandLineApplication app)
        {
            app.Description = Description;
            app.HelpOption("-?|-h|--help");

            foreach (var commandOption in GetCommandOptions())
            {
                app.Option(commandOption.Template, commandOption.Description, commandOption.OptionType);
            }

            app.OnExecute(() => OnExecute(app));
        }

        protected abstract IEnumerable<CommandOption> GetCommandOptions();

        protected abstract int OnExecute(CommandLineApplication app);
    }
}