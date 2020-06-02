using Yaclip.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Yaclip
{
    public interface ICommandBuilder<T>
    {
        ICommandBuilder<T> Option<TOpt>(Expression<Func<T, TOpt>> expr, Action<IOptionBuilder<TOpt>> builder);
        ICommandBuilder<T> Argument<TArg>(Expression<Func<T, TArg>> expr, Action<IArgumentBuilder<TArg>> builder);
        ICommandBuilder<T> Callback(Action<T> action);
        ICommandBuilder<T> Description(string description);
    }

    internal class CommandBuilder<T> : ICommandBuilder<T>
    {
        private readonly string Name;

        private readonly IList<Option> Options = new List<Option>();
        private readonly IList<Argument> Arguments = new List<Argument>();
        private string? Description;
        private Action<T>? CallbackAction;

        public CommandBuilder(string name)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        ICommandBuilder<T> ICommandBuilder<T>.Option<TOpt>(Expression<Func<T, TOpt>> expr, Action<IOptionBuilder<TOpt>> builder)
        {
            var b = new OptionBuilder<TOpt>(expr.Body);
            builder(b);

            var opt = b.Build();

            if (opt.LongName != null && Options.Any(o => o.LongName == opt.LongName))
                throw new BuilderException($"Duplicate option long name '{opt.LongName}'");

            if (opt.ShortName != null && Options.Any(o => o.ShortName == opt.ShortName))
                throw new BuilderException($"Duplicate option short name '{opt.ShortName}'");

            Options.Add(opt);
            return this;
        }
        
        ICommandBuilder<T> ICommandBuilder<T>.Argument<TArg>(Expression<Func<T, TArg>> expr, Action<IArgumentBuilder<TArg>> builder)
        {
            var b = new ArgumentBuilder<TArg>(expr.Body);
            builder(b);

            var arg = b.Build();

            if (arg.Required && Arguments.Any(o => !o.Required))
                throw new BuilderException("Required arguments must appear before optional ones");

            Arguments.Add(arg);
            return this;
        }

        ICommandBuilder<T> ICommandBuilder<T>.Callback(Action<T> action)
        {
            if (CallbackAction != null)
                throw new BuilderException("Callback already defined");

            CallbackAction = action;
            return this;
        }

        ICommandBuilder<T> ICommandBuilder<T>.Description(string description)
        {
            Description = description;
            return this;
        }

        public Command<T> Build()
        {
            if (CallbackAction == null)
                throw new BuilderException("Missing callback action");

            return new Command<T>(Name, Description, Options.ToArray(), Arguments.ToArray(), typeof(T), CallbackAction);
        }
    }
}
