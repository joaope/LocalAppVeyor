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
                    onOutputDataReceived(powerShell.Streams.Information[args.Index].MessageData?.ToString());
                };

                powerShell.Streams.Error.DataAdded += (sender, args) =>
                {
                    var error = powerShell.Streams.Error[args.Index];
                    var msg =
                        $"STACKTRACE: {(string.IsNullOrEmpty(error.ScriptStackTrace) ? "<no stacktrace>" : error.ScriptStackTrace)}. ";

                    if (error.ErrorDetails != null)
                    {
                        msg +=
                            $"MESSAGE: {(string.IsNullOrEmpty(error.ErrorDetails.Message) ? "<no error message>" : error.ErrorDetails.Message)}. ";
                        msg +=
                            $"RECOMMENDED ACTION: {(string.IsNullOrEmpty(error.ErrorDetails.RecommendedAction) ? "<no recommended action>" : error.ErrorDetails.RecommendedAction)}.";
                    }

                    onErrorDataReceived(msg);
                    success = false;
                };

                powerShell.Invoke();

                return success;
            }
        }
    }
}