using System;
using System.Management.Automation;

namespace LocalAppVeyor.Engine.Internal
{
    internal static class PowerShellScriptExecuter
    {
        public static bool Execute(
            string script,
            Action<string> onOutputDataReceived,
            Action<string> onErrorDataReceived)
        {
            using (var powerShell = PowerShell.Create())
            {
                var success = true;

                powerShell.AddScript(script);

                powerShell.Streams.Information.DataAdded += (sender, args) =>
                {
                    onOutputDataReceived(powerShell.Streams.Information[args.Index].MessageData.ToString());
                };

                powerShell.Streams.Error.DataAdded += (sender, args) =>
                {
                    onErrorDataReceived(powerShell.Streams.Error[args.Index].ErrorDetails.Message);
                    success = false;
                };

                powerShell.Invoke();

                return success;
            }
        }
    }
}