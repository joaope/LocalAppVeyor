using LocalAppVeyor.Configuration.Model;

namespace LocalAppVeyor.Configuration.Reader
{
    public interface IBuildConfigurationReader
    {
        BuildConfiguration GetBuildConfiguration();
    }
}
