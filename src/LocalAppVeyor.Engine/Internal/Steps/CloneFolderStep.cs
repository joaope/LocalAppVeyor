using System;
using LocalAppVeyor.Engine.IO;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal class CloneFolderStep : IInternalEngineStep
    {
        private readonly FileSystem fileSystem;

        public CloneFolderStep(FileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public bool Execute(ExecutionContext executionContext)
        {
            executionContext.Outputter.Write($"Cloning '{executionContext.RepositoryDirectory}' in to '{executionContext.CloneDirectory}'...");

            try
            {
                Clone(executionContext.RepositoryDirectory, executionContext.CloneDirectory);

                executionContext.Outputter.Write("Cloning finished.");
                return true;
            }
            catch (Exception e)
            {
                executionContext.Outputter.WriteError($"Error while cloning folder: {e.Message}");
                return false;
            }
        }

        private void Clone(string source, string destination)
        {
            var dirSource = new DirectoryInfo(source);
            var dirDestination = new DirectoryInfo(destination);

            if (!dirDestination.Exists)
            {
                fileSystem.Directory.CreateDirectory(dirDestination.FullName);
            }

            // empty destination
            foreach (var fileInfo in dirDestination.GetFiles())
            {
                fileInfo.Delete();
            }

            foreach (var directoryInfo in dirDestination.GetDirectories())
            {
                directoryInfo.Delete(true);
            }

            CopyAll(dirSource, dirDestination);
        }

        private void CopyAll(DirectoryInfo source, DirectoryInfo destination)
        {
            // copy each file into destination
            foreach (var file in source.GetFiles())
            {
                file.CopyTo(fileSystem.Path.Combine(destination.FullName, file.Name), true);
            }

            // copy each subdirectory using recursion
            foreach (var diSourceSubDir in source.GetDirectories())
            {
                var nextTargetSubDir = destination.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }
}
