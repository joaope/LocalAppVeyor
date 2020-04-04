using System;
using YamlDotNet.Core;

namespace LocalAppVeyor.Engine
{
    public class LocalAppVeyorException : Exception
    {
        public Mark Start { get; }

        public Mark End { get; }

        public LocalAppVeyorException()
        {
        }

        public LocalAppVeyorException(string message)
            : base(message)
        {
        }
        
        public LocalAppVeyorException(string message, Exception innerException)
            : base(message, innerException)
        {
            if (!(innerException is YamlException yamlEx))
            {
                return;
            }

            Start = new Mark(yamlEx.Start.Line, yamlEx.Start.Column);
            End = new Mark(yamlEx.End.Line, yamlEx.End.Column);
        }

        public sealed class Mark
        {
            public int Line { get; }

            public int Column { get; }

            public Mark(int line, int column)
            {
                Line = line;
                Column = column;
            }

            public override string ToString()
            {
                return $"(Line: {Line}, Column: {Column})";
            }
        }
    }
}