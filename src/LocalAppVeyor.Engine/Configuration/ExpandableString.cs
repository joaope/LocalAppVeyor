using System;
using System.Text.RegularExpressions;

namespace LocalAppVeyor.Engine.Configuration
{
    public struct ExpandableString
    {
        private static readonly Regex VarPattern = new Regex(@"\$\([\w-]+\)", RegexOptions.Compiled);

        private readonly string _internalStr;

        public static ExpandableString Empty => new ExpandableString(string.Empty);

        public ExpandableString(string str)
        {
            _internalStr = str;
        }

        public static implicit operator ExpandableString(string str)
        {
            return new ExpandableString(str);
        }

        public static implicit operator string(ExpandableString expandable)
        {
            if (string.IsNullOrEmpty(expandable._internalStr))
            {
                return expandable._internalStr;
            }
            
            return VarPattern
                .Replace(expandable._internalStr, m => Environment.GetEnvironmentVariable(m.Value.Substring(2, m.Value.Length - 3)))
                .Replace("{build}", "0")
                .Replace("{version}", Environment.GetEnvironmentVariable("APPVEYOR_BUILD_VERSION"));
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case null:
                    return false;
                case string s:
                    return this == s;
                case ExpandableString expandableString:
                    return expandableString._internalStr == _internalStr;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return _internalStr.GetHashCode();
        }

        public override string ToString()
        {
            return this;
        }
    }
}
