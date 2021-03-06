﻿using System;
using System.Linq.Expressions;

namespace Yaclip
{
    public interface IOptionBuilder<T>
    {
        IOptionBuilder<T> ValueName(string name);
        IOptionBuilder<T> Description(string desc);
    }

    internal class OptionBuilder<T> : IOptionBuilder<T>
    {
        private char? ShortName;
        private string? LongName, ValueName, Description;

        private readonly Expression Expression;

        public OptionBuilder(Expression expr, char? shortName, string? longName)
        {
            this.Expression = expr ?? throw new ArgumentNullException(nameof(expr));
            this.ShortName = shortName;
            this.LongName = longName;
        }

        IOptionBuilder<T> IOptionBuilder<T>.ValueName(string name)
        {
            ValueName = name;
            return this;
        }

        IOptionBuilder<T> IOptionBuilder<T>.Description(string desc)
        {
            Description = desc;
            return this;
        }

        public Option Build()
        {
            if (ShortName == null && LongName == null)
                throw new BuilderException("The option needs at least either a short or a long name");

            return new Option(ShortName, LongName, Description, Expression, typeof(T), ValueName);
        }
    }
}
