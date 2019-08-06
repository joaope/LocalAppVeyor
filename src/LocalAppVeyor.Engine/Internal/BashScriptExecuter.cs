using System;
using System.Diagnostics;

namespace LocalAppVeyor.Engine.Internal
{
    internal static class BashScriptExecuter
    {
        public static bool Execute(
            string script,
            Action<string> onOutputDataReceived,
            Action<string> onErrorDataReceived)
        {
            if (string.IsNullOrEmpty(script))
            {
                return true;
            }

            using (var process = Process.Start(new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{script.Replace("\"", "\\\"")}\"",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
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