using YamlDotNet.Serialization;

namespace LocalAppVeyor.Engine.Configuration.Reader.Internal.Model
{
    internal class InternalMatrix
    {
        [YamlMember(Alias = "fast_finish")]
        public bool IsFastFinish { get; set; }

        public Matrix ToMatrix()
        {
            return new Matrix(IsFastFinish);
        }
    }
}