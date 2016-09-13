namespace LocalAppVeyor.Engine.Configuration
{
    public class Build
    {
        public bool IsAutomaticBuildOff { get; }

        public bool IsParallel { get; }

        public string SolutionFile { get; }
        
        public BuildVerbosity Verbosity { get; }

        public Build()
            : this(false, false, null, BuildVerbosity.Normal)
        {
        }

        public Build(
            bool isAutomaticBuildOff,
            bool isParallel, 
            string solutionFile, 
            BuildVerbosity verbosity)
        {
            IsAutomaticBuildOff = isAutomaticBuildOff;
            IsParallel = isParallel;
            SolutionFile = solutionFile;
            Verbosity = verbosity;
        }
    }
}
