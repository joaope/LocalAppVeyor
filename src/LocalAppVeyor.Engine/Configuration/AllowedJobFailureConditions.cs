using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LocalAppVeyor.Engine.Configuration
{
    public class AllowedJobFailureConditions
    {
        public string OperatingSystem { get; }

        public string Configuration { get; }

        public string Platform { get; }

        public string TestCategory { get; }

        public ReadOnlyCollection<Variable> Variables { get; }

        public AllowedJobFailureConditions(
            string operatingSystem, 
            string configuration, 
            string platform, 
            string testCategory, 
            IEnumerable<Variable> variables)
        {
            OperatingSystem = operatingSystem;
            Configuration = configuration;
            Platform = platform;
            TestCategory = testCategory;
            Variables = new ReadOnlyCollection<Variable>(variables?.ToList() ?? new List<Variable>());
        }

        public bool AreConditionsMetForJob(MatrixJob job)
        {
            if (job == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(OperatingSystem))
            {
                if (OperatingSystem != job.OperatingSystem)
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(Configuration))
            {
                if (Configuration != job.Configuration)
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(Platform))
            {
                if (Platform != job.Platform)
                {
                    return false;
                }
            }

            if (Variables.Count > 0)
            {
                if (!Variables.All(v => job.Variables.Contains(v)))
                {
                    return false;
                }
            }

            return true;
        }
    }
}