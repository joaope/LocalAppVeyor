using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LocalAppVeyor.Engine.Configuration
{
    public class Matrix
    {
        public bool IsFastFinish { get; }

        public ReadOnlyCollection<AllowedJobFailureConditions> AllowedFailures { get; }

        public Matrix()
            : this(false, new AllowedJobFailureConditions[0])
        {
        }

        public Matrix(
            bool isFastFinish,
            IEnumerable<AllowedJobFailureConditions> allowedFailures)
        {
            IsFastFinish = isFastFinish;
            AllowedFailures = new ReadOnlyCollection<AllowedJobFailureConditions>(allowedFailures?.ToList() ?? new List<AllowedJobFailureConditions>());
        }
    }
}
