using YamlDotNet.Serialization;

namespace LocalAppVeyor.Engine.Configuration.Reader.Internal.Model
{
    internal class InternalMatrix
    {
        [YamlMember(Alias = "fast_finish")]
        public bool IsFastFinish { get; set; }

        [YamlMember(Alias = "allow_failures")]
        public AllowedFailuresCollection AllowedFailures { get; set; }

        public Matrix ToMatrix()
        {
            return new Matrix(IsFastFinish, AllowedFailures);
        }
    }
}