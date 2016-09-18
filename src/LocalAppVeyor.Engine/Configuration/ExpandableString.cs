using System;
using System.Text.RegularExpressions;

namespace LocalAppVeyor.Engine.Configuration
{
    public struct ExpandableString
    {
        private static readonly Regex VarPattern = new Regex(@"\$\([\w-]+\)", RegexOptions.Compiled);

        private readonly string internalStr;

        public ExpandableString(string str)
        {
            internalStr = str;
        }

        public static implicit operator ExpandableString(string str)
        {
            return new ExpandableString(str);
        }

        public static implicit operator string(ExpandableString expandable)
        {
            if (string.IsNullOrEmpty(expandable.internalStr))
            {
                return expandable.internalStr;
            }
            
            return VarPattern
                .Replace(expandable.internalStr, m => Environment.GetEnvironmentVariable(m.Value.Substring(2, m.Value.Length - 3)))
                .Replace("{build}", "0")
                .Replace("{version}", Environment.GetEnvironmentVariable("APPVEYOR_BUILD_VERSION"));
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var s = obj as string;
            if (s != null)
            {
                return this == s;
            }
            
            if (obj is ExpandableString)
            {
                return ((ExpandableString)obj).internalStr == internalStr;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return internalStr.GetHashCode();
        }

        public override string ToString()
        {
            return this;
        }
    }
}
