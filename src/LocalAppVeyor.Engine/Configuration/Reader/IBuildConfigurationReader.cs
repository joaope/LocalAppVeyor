using LocalAppVeyor.Engine.Configuration.Model;

namespace LocalAppVeyor.Engine.Configuration.Reader
{
    public interface IBuildConfigurationReader
    {
        BuildConfiguration GetBuildConfiguration();
    }
}
