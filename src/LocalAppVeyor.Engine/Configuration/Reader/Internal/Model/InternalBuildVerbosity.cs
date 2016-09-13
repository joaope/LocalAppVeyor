using System;

namespace LocalAppVeyor.Engine.Configuration.Reader.Internal.Model
{
    internal enum InternalBuildVerbosity
    {
        Quiet,
        Minimal,
        Normal,
        Detailed
    }

    internal static class InternalBuildVerbosityExtensions
    {
        public static BuildVerbosity ToBuildVerbosity(this InternalBuildVerbosity buildVerbosity)
        {
            switch (buildVerbosity)
            {
                case InternalBuildVerbosity.Quiet:
                    return BuildVerbosity.Quiet;
                case InternalBuildVerbosity.Minimal:
                    return BuildVerbosity.Minimal;
                case InternalBuildVerbosity.Normal:
                    return BuildVerbosity.Normal;
                case InternalBuildVerbosity.Detailed:
                    return BuildVerbosity.Detailed;
                default:
                    throw new ArgumentOutOfRangeException(nameof(buildVerbosity), buildVerbosity, null);
            }
        }
    }
}