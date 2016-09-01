using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace LocalAppVeyor.Pipeline.Steps.Internal
{
    internal static class BatchScriptExecuter
    {
        public static bool Execute(
            string workingDirectory,
            string script, 
            Action<string> onOutputDataReceived,
            Action<string> onErrorDataReceived,
            ReadOnlyDictionary<string, string> environmentVariables)
        {
            var batchFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.bat");

            var scriptBuilder = new StringBuilder();

            if (environmentVariables != null && environmentVariables.Count > 0)
            {
                foreach (var variable in environmentVariables)
                {
                    scriptBuilder.AppendLine($"@set {variable.Key}={variable.Value}");
                }
            }

            scriptBuilder.Append(script);

            using (var fileStream = new FileStream(batchFile, FileMode.Create, FileAccess.ReadWrite))
            using (var writer = new StreamWriter(fileStream))
            {
                writer.Write(scriptBuilder.Length > 0 ? scriptBuilder.ToString() : string.Empty);
            }

            using (var process = Process.Start(new ProcessStartInfo("cmd.exe", $"/c {batchFile}")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WorkingDirectory = workingDirectory
            }))
            {
                process.OutputDataReceived += (s, e) =>
                {
                    onOutputDataReceived?.Invoke(e.Data);
                };
                process.ErrorDataReceived += (s, e) =>
                {
                    onErrorDataReceived?.Invoke(e.Data);
                };

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();

                return process.ExitCode == 0;
            }
        }
    }
}
