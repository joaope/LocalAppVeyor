using System;
using System.Management.Automation;

namespace LocalAppVeyor.Engine.Pipeline.Internal
{
    internal static class PowerShellScriptExecuter
    {
        public static bool Execute(
            string workingDirectory,
            string script,
            Action<string> onOutputDataReceived,
            Action<string> onErrorDataReceived)
        {
            using (var powerShell = PowerShell.Create())
            {
                var errors = false;

                powerShell.AddScript(script);

                powerShell.Streams.Information.DataAdded += (sender, args) =>
                {
                    onOutputDataReceived(powerShell.Streams.Information[args.Index].MessageData.ToString());
                };

                powerShell.Streams.Error.DataAdded += (sender, args) =>
                {
                    onOutputDataReceived(powerShell.Streams.Error[args.Index].ErrorDetails.Message);
                    errors = true;
                };

                powerShell.Invoke();

                return errors;
            }
        }
    }
}