namespace LocalAppVeyor.Configuration.Model
{
    public class Matrix
    {
        public bool IsFastFinish { get; }

        public Matrix()
            : this(false)
        {
        }

        public Matrix(bool isFastFinish)
        {
            IsFastFinish = isFastFinish;
        }
    }
}
