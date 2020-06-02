using System;
using System.Linq.Expressions;

namespace Yaclip.Builders
{
    public interface IArgumentBuilder<T>
    {
        IArgumentBuilder<T> Required(bool required);
        IArgumentBuilder<T> Name(string name);
    }

    internal class ArgumentBuilder<T> : IArgumentBuilder<T>
    {
        private readonly Expression MemberExpression;

        private bool Required;
        private string? Name;

        public ArgumentBuilder(Expression memberExpression)
        {
            this.MemberExpression = memberExpression ?? throw new ArgumentNullException(nameof(memberExpression));
        }

        IArgumentBuilder<T> IArgumentBuilder<T>.Required(bool required)
        {
            Required = required;
            return this;
        }

        IArgumentBuilder<T> IArgumentBuilder<T>.Name(string name)
        {
            Name = name;
            return this;
        }

        public Argument Build()
        {
            return new Argument(typeof(T), MemberExpression, Required, Name ?? "arg");
        }
    }
}
