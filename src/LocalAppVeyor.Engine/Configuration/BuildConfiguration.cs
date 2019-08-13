using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LocalAppVeyor.Engine.Configuration
{
    public class BuildConfiguration
    {
        public static BuildConfiguration Default => new BuildConfiguration();
        
        public ExpandableString Version { get; }

        public ScriptBlock InitializationScript { get; }
        
        public ExpandableString CloneFolder { get; }

        public Matrix Matrix { get; }

        public ScriptBlock InstallScript { get; }

        public AssemblyInfo AssemblyInfo { get; }

        public ReadOnlyCollection<string> OperatingSystems { get; }

        public EnvironmentVariables EnvironmentVariables { get; }

        public ReadOnlyCollection<string> Platforms { get; }

        public ReadOnlyCollection<string> Configurations { get; }

        public Build Build { get; }

        public ScriptBlock BeforeBuildScript { get; }

        public ScriptBlock BuildScript { get; }

        public ScriptBlock AfterBuildScript { get; }

        public ScriptBlock TestScript { get; }

        public ScriptBlock OnSuccessScript { get; }

        public ScriptBlock OnFailureScript { get; }

        public ScriptBlock OnFinishScript { get; }

        private string[] _skipSteps = new string[0];

        public string[] SkipSteps
        {
            get => _skipSteps;
            set => _skipSteps = value ?? new string[0];
        }

        public BuildConfiguration()
            : this(
                  null, 
                  null, 
                  null, 
                  null,
                  null, 
                  new string[0], 
                  null, 
                  null, 
                  new string[0],
                  new string[0], 
                  null, 
                  null, 
                  null, 
                  null,
                  null,
                  null,
                  null,
                  null)
        {
        }

        public BuildConfiguration(
            ExpandableString version, 
            ScriptBlock initializationScript,
            ExpandableString cloneFolder, 
            ScriptBlock installScript,
            AssemblyInfo assemblyInfo,
            IEnumerable<string> operatingSystems,
            EnvironmentVariables environmentVariables,
            Matrix matrix,
            IEnumerable<string> platforms,
            IEnumerable<string> configurations,
            Build build, 
            ScriptBlock beforeBuildScript,
            ScriptBlock buildScript,
            ScriptBlock afterBuildScript,
            ScriptBlock testScript,
            ScriptBlock onSuccess,
            ScriptBlock onFailure,
            ScriptBlock onFinish)
        {
            Version = version;
            InitializationScript = initializationScript ?? new ScriptBlock();
            CloneFolder = cloneFolder;
            InstallScript = installScript ?? new ScriptBlock();
            AssemblyInfo = assemblyInfo ?? new AssemblyInfo();
            OperatingSystems = new ReadOnlyCollection<string>(operatingSystems?.ToList() ?? new List<string>());
            EnvironmentVariables = environmentVariables ?? new EnvironmentVariables();
            Matrix = matrix ?? new Matrix();
            Platforms = new ReadOnlyCollection<string>(platforms?.ToList() ?? new List<string>());
            Configurations = new ReadOnlyCollection<string>(configurations?.ToList() ?? new List<string>());
            Build = build ?? new Build();
            BeforeBuildScript = beforeBuildScript ?? new ScriptBlock();
            BuildScript = buildScript ?? new ScriptBlock();
            AfterBuildScript = afterBuildScript ?? new ScriptBlock();
            TestScript = testScript ?? new ScriptBlock();
            OnSuccessScript = onSuccess ?? new ScriptBlock();
            OnFailureScript = onFailure ?? new ScriptBlock();
            OnFinishScript = onFinish ?? new ScriptBlock();
        }
    }
}
