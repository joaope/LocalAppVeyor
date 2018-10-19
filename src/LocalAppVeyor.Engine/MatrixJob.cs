using System.Collections.Generic;
using System.Linq;
using LocalAppVeyor.Engine.Configuration;

namespace LocalAppVeyor.Engine
{
    public class MatrixJob
    {
        public string OperatingSystem { get; }

        public string Platform { get; }

        public string Configuration { get; }

        public IReadOnlyCollection<Variable> Variables { get; }

        private string _name;

        public string Name
        {
            get
            {
                if (_name != null)
                {
                    return _name;
                }

                if (string.IsNullOrEmpty(OperatingSystem) &&
                    string.IsNullOrEmpty(Platform) &&
                    string.IsNullOrEmpty(Configuration) &&
                    Variables.Count == 0)
                {
                    return "Default Job";
                }

                var nameParts = new List<string>();

                if (!string.IsNullOrEmpty(OperatingSystem))
                {
                    nameParts.Add($"OS: {OperatingSystem}");
                }

                if (Variables.Count > 0)
                {
                    nameParts.Add($"Environment: {string.Join(", ", Variables.Select(v => v.ToString()))}");
                }

                if (!string.IsNullOrEmpty(Configuration))
                {
                    nameParts.Add($"Configuration: {Configuration}");
                }

                if (!string.IsNullOrEmpty(Platform))
                {
                    nameParts.Add($"Platform: {Platform}");
                }

                return _name = string.Join("; ", nameParts);
            }
        }

        public MatrixJob(
            string operatingSystem,
            IReadOnlyCollection<Variable> variables,
            string configuration,
            string platform)
        {
            OperatingSystem = operatingSystem;
            Platform = platform;
            Configuration = configuration;
            Variables = variables ?? new Variable[0];
        }

        public override string ToString()
        {
            return Name;
        }
    }
}