using System;

namespace Yaclip
{
    internal interface ICommand
    {
        string[] Name { get; }
        string? Description { get; }
        Func<object> Factory { get; }
        Type ObjectType { get; }
        Option[] Options { get; }
        Argument[] Arguments { get; }

        void Run(object obj);
    }

    internal static class CommandExtensions
    {
        public static string FullName(this ICommand cmd) => string.Join(" ", cmd.Name);
    }

    internal class Command<T> : ICommand
    {
        public string[] Name { get; }
        public string? Description { get; }
        public Func<object> Factory { get; }
        public Option[] Options { get; }
        public Argument[] Arguments { get; }
        public Type ObjectType { get; }
        public Action<T> Callback { get; }

        public Command(string[] name, string? description, Func<object> factory, Option[] options, Argument[] arguments, Type objectType, Action<T> callback)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Description = description;
            this.Factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
            this.Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            this.ObjectType = objectType ?? throw new ArgumentNullException(nameof(objectType));
            this.Callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        public void Run(object obj)
        {
            Callback((T)obj);
        }
    }
}
