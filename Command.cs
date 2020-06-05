using System;

namespace Yaclip
{
    internal class Command
    {
        public virtual string[] Name { get; }
        public virtual string? Description { get; }
        public virtual Func<object> Factory { get; }
        public virtual Option[] Options { get; }
        public virtual Argument[] Arguments { get; }
        public virtual Type ObjectType { get; }
        public virtual Action<object> Callback { get; }

        public string FullName => string.Join(" ", Name);

#nullable disable
        protected Command()
        {
        }
#nullable enable

        public Command(string[] name, string? description, Func<object> factory, Option[] options, Argument[] arguments, Type objectType, Action<object> callback)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Description = description;
            this.Factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
            this.Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            this.ObjectType = objectType ?? throw new ArgumentNullException(nameof(objectType));
            this.Callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        public virtual void Run(object obj)
        {
            Callback(obj);
        }
    }
}
