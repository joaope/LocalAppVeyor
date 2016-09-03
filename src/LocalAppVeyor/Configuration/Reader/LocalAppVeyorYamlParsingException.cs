namespace LocalAppVeyor.Configuration.Reader
{
    public class LocalAppVeyorYamlParsingException : LocalAppVeyorException
    {
        public int StartLine { get; }

        public int StartColumn { get; }

        public int EndLine { get; }

        public int EndColumn { get; }

        public LocalAppVeyorYamlParsingException(string message)
            : this(message, -1, -1, -1, -1)
        {
        }

        public LocalAppVeyorYamlParsingException(
            string message,
            int startLine, 
            int startColumn,
            int endLine,
            int endColumn)
            : base(message)
        {
            StartLine = startLine;
            StartColumn = startColumn;
            EndLine = endLine;
            EndColumn = endColumn;
        }
    }
}
