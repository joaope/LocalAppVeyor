using LocalAppVeyor.Configuration.Model;

namespace LocalAppVeyor.Configuration.Readers
{
    public interface IBuildConfigurationReader
    {
        BuildConfiguration GetBuildConfiguration();
    }
}
