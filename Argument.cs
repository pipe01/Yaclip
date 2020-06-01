using System;
using System.Linq.Expressions;

namespace LogicCommandLineParser
{
    internal class Argument
    {
        public Type Type { get; }
        public Expression MemberExpression { get; }
        public bool Required { get; }
        public string Name { get; }

        public Argument(Type type, Expression memberExpression, bool required, string name)
        {
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
            this.MemberExpression = memberExpression ?? throw new ArgumentNullException(nameof(memberExpression));
            this.Required = required;
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
