using System;
using YamlDotNet.Core;

namespace LocalAppVeyor.Engine
{
    public class LocalAppVeyorException : Exception
    {
        public LocalAppVeyorException()
        {
        }

        public LocalAppVeyorException(string message)
            : base(message)
        {
        }
        
        public LocalAppVeyorException(string message, Exception innerException)
            : base(GetDetailedMessage(message, innerException), innerException)
        {
        }

        private static string GetDetailedMessage(string message, Exception innerException)
        {
            return innerException is YamlException yamlEx
                ? "Error while parsing YAML " +
                  $"<Line: {yamlEx.Start.Line}, Column: {yamlEx.Start.Column}> to " +
                  $"<Line: {yamlEx.End.Line}, Column: {yamlEx.End.Column}>"
                : message;
        }
    }
}