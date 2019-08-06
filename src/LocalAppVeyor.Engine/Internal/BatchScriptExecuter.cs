using System;
using System.Diagnostics;
using System.IO.Abstractions;

namespace LocalAppVeyor.Engine.Internal
{
    internal static class BatchScriptExecuter
    {
        public static bool Execute(
            IFileSystem fileSystem,
            string workingDirectory,
            string script, 
            Action<string> onOutputDataReceived,
            Action<string> onErrorDataReceived)
        {
            if (string.IsNullOrEmpty(script))
            {
                return true;
            }

            var batchFile = fileSystem.Path.Combine(fileSystem.Path.GetTempPath(), $"{Guid.NewGuid()}.bat");
            fileSystem.File.WriteAllText(batchFile, script);

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
