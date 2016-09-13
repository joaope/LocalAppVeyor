using System;
using LocalAppVeyor.Engine.Configuration;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;

namespace LocalAppVeyor.Engine.Internal
{
    internal class PipelineOutputterMsBuildLogger : ConsoleLogger
    {
        public PipelineOutputterMsBuildLogger(BuildVerbosity verbosity, IPipelineOutputter outputter)
            : base(
                  TransformToLoggerVerbosity(verbosity),
                  outputter.Write,
                  outputter.SetColor,
                  outputter.ResetColor
                  )
        {
        }

        private static LoggerVerbosity TransformToLoggerVerbosity(BuildVerbosity verbosity)
        {
            switch (verbosity)
            {
                case BuildVerbosity.Quiet:
                    return LoggerVerbosity.Quiet;
                case BuildVerbosity.Minimal:
                    return LoggerVerbosity.Minimal;
                case BuildVerbosity.Normal:
                    return LoggerVerbosity.Normal;
                case BuildVerbosity.Detailed:
                    return LoggerVerbosity.Detailed;
                default:
                    throw new ArgumentOutOfRangeException(nameof(verbosity), verbosity, null);
            }
        }
    }
}