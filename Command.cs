using System;

namespace Yaclip
{
    internal abstract class Command
    {
        public string? Description { get; protected set; }
#nullable disable
        public string Name { get; protected set; }
        public Option[] Options { get; protected set; }
        public Argument[] Arguments { get; protected set; }
        public Type ObjectType { get; protected set; }
#nullable enable

        public abstract void Run(object obj);
    }

    internal class Command<T> : Command
    {
        public Action<T> Callback { get; }

        public Command(string name, string? description, Option[] options, Argument[] arguments, Type objectType, Action<T>? callback)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Options = options;
            this.Arguments = arguments;
            this.ObjectType = objectType ?? throw new ArgumentNullException(nameof(objectType));
            this.Callback = callback ?? throw new ArgumentNullException(nameof(callback));
            this.Description = description;
        }

        public override void Run(object obj)
        {
            Callback((T)obj);
        }
    }
}
