using System;
using System.Linq.Expressions;

namespace LogicCommandLineParser
{
    internal class Option
    {
        public char? ShortName { get; }
        public string? LongName { get; }
        public string? ValueName { get; }
        public string? Description { get; }
        public Type ValueType { get; }
        public Expression MemberExpression { get; }

        public Option(char? shortName, string? longName, string? description, Expression memberExpression, Type valueType, string? valueName)
        {
            this.ShortName = shortName;
            this.LongName = longName;
            this.Description = description;
            this.MemberExpression = memberExpression;
            this.ValueType = valueType;
            this.ValueName = valueName;
        }

        public override string ToString()
        {
            if (ShortName != null && LongName != null)
                return $"{FormatShort()} | {FormatLong()}";
            else if (ShortName != null)
                return FormatShort();
            else if (LongName != null)
                return FormatLong();
            else
                return "";
        }

        private string FormatShort()
        {
            var str = $"-{ShortName}";

            if (ValueType != typeof(bool))
                str += $"=<{ValueName ?? "value"}>";

            return str;
        }

        private string FormatLong()
        {
            var str = $"--{LongName}";

            if (ValueType != typeof(bool))
                str += $"=<{ValueName ?? "value"}>";

            return str;
        }
    }
}
