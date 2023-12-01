using System;
using System.Management.Automation;

namespace LocalAppVeyor.Engine.Internal;

internal static class PowerShellScriptExecuter
{
    public static bool Execute(
        string script,
        Action<InformationRecord> onOutputDataReceived,
        Action<ErrorRecord> onErrorDataReceived)
    {
        using var powerShell = PowerShell.Create();
        var success = true;

        powerShell.AddScript(script);

        powerShell.Streams.Information.DataAdded += (sender, args) =>
        {
            onOutputDataReceived(powerShell.Streams.Information[args.Index]);
        };

        powerShell.Streams.Error.DataAdded += (sender, args) =>
        {
            onErrorDataReceived(powerShell.Streams.Error[args.Index]);
            success = false;
        };

        powerShell.Invoke();

        return success;
    }
}