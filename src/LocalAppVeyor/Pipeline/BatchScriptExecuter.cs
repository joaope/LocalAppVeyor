using System;
using System.Diagnostics;
using System.IO;

namespace LocalAppVeyor.Pipeline
{
    internal static class BatchScriptExecuter
    {
        public static bool Execute(
            string workingDirectory,
            string script, 
            Action<string> onOutputDataReceived,
            Action<string> onErrorDataReceived)
        {
            if (string.IsNullOrEmpty(script))
            {
                return true;
            }

            var batchFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.bat");

            using (var fileStream = new FileStream(batchFile, FileMode.Create, FileAccess.ReadWrite))
            using (var writer = new StreamWriter(fileStream))
            {
                writer.Write(script);
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
