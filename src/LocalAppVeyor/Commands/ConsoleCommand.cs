using System.Threading.Tasks;
using LocalAppVeyor.Engine;
using McMaster.Extensions.CommandLineUtils;

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

            SetUpAdditionalCommandOptions(app);

            app.OnExecuteAsync(token => OnExecute(app));
        }

        protected virtual void SetUpAdditionalCommandOptions(CommandLineApplication app)
        {
        }

        protected abstract Task<int> OnExecute(CommandLineApplication app);
    }
}