namespace LocalAppVeyor.Engine.Configuration
{
    public class Build
    {
        public bool IsAutomaticBuildOff { get; }

        public bool IsParallel { get; }

        public ExpandableString SolutionFile { get; }
        
        public BuildVerbosity Verbosity { get; }

        public Build()
            : this(true, false, null, BuildVerbosity.Normal)
        {
        }

        public Build(
            bool isAutomaticBuildOff,
            bool isParallel, 
            ExpandableString solutionFile, 
            BuildVerbosity verbosity)
        {
            IsAutomaticBuildOff = isAutomaticBuildOff;
            IsParallel = isParallel;
            SolutionFile = solutionFile;
            Verbosity = verbosity;
        }
    }
}
