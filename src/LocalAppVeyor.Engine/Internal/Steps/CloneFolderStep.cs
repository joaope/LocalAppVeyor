using System.IO.Abstractions;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal class CloneFolderStep : IEngineStep
    {
        public string Name => "clone_folder";

        private readonly IFileSystem _fileSystem;

        public CloneFolderStep(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public bool Execute(ExecutionContext executionContext)
        {
            executionContext.Outputter.Write($"Cloning '{executionContext.RepositoryDirectory}' in to '{executionContext.CloneDirectory}'...");
            Clone(executionContext.RepositoryDirectory, executionContext.CloneDirectory);
            executionContext.Outputter.Write("Cloning finished.");

            return true;
        }

        private void Clone(string source, string destination)
        {
            var dirSource = _fileSystem.DirectoryInfo.FromDirectoryName(source);
            var dirDestination = _fileSystem.DirectoryInfo.FromDirectoryName(destination);

            if (!dirDestination.Exists)
            {
                _fileSystem.Directory.CreateDirectory(dirDestination.FullName);
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

        private void CopyAll(IDirectoryInfo source, IDirectoryInfo destination)
        {
            // copy each file into destination
            foreach (var file in source.GetFiles())
            {
                file.CopyTo(_fileSystem.Path.Combine(destination.FullName, file.Name), true);
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
