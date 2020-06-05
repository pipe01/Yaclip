using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Yaclip
{
    internal class HelpCommand : Command
    {
        private class HelpOptions
        {
            public string[]? Command { get; set; }
        }

        public override string[] Name { get; } = new[] { "help" };
        public override string? Description { get; } = "Provides help about the usage of the program and its commands.";
        public override Func<object> Factory { get; } = () => new HelpOptions();
        public override Type ObjectType { get; } = typeof(HelpOptions);
        public override Option[] Options { get; } = Array.Empty<Option>();
        public override Argument[] Arguments { get; } = new[]
        {
            new Argument(typeof(string[]), Expression.Property(Expression.Variable(typeof(HelpOptions)), nameof(HelpOptions.Command)), false, "cmd"),
        };

        private readonly YaclipApp App;

        public HelpCommand(YaclipApp app)
        {
            this.App = app ?? throw new ArgumentNullException(nameof(app));
        }

        public override void Run(object obj)
        {
            var opts = (HelpOptions)obj;

            if (opts.Command == null)
            {
                HelpForCommands(App.Commands);
            }
            else
            {
                var cmds = App.Commands.Where(o => o.Name.StartsWith(opts.Command)).ToArray();

                if (cmds.Length == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Unknown command '{string.Join(" ", opts.Command)}'");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else if (cmds.Length == 1)
                {
                    HelpForCommand(cmds[0]);
                }
                else
                {
                    HelpForCommands(cmds);
                }
            }
        }

        private void HelpForCommands(IEnumerable<Command> cmds)
        {
            var str = new StringBuilder();
            AppendHeader(str);

            str.AppendLine("Usage:");

            foreach (var cmd in cmds)
            {
                str.Append("  ");
                AppendCommand(str, cmd);
            }

            Console.WriteLine(str.ToString());
        }

        private void HelpForCommand(Command cmd)
        {
            var str = new StringBuilder();
            AppendHeader(str);

            if (cmd.Description != null)
                str.AppendLine(cmd.Description).AppendLine();

            str.Append("Usage: ");
            AppendCommand(str, cmd);

            if (cmd.Options.Length > 0)
            {
                str.AppendLine().AppendLine("Options:");

                int maxLength = cmd.Options.Max(o => o.ToString().Length);

                foreach (var opt in cmd.Options)
                {
                    string optStr = opt.ToString().Replace(" |", ",");
                    str.Append("  ").Append(optStr);

                    if (opt.Description != null)
                        str.Append(new string(' ', maxLength - optStr.Length + 2)).Append(opt.Description);

                    str.AppendLine();
                }
            }

            Console.WriteLine(str.ToString());
        }

        private void AppendHeader(StringBuilder str)
        {
            str.Append(App.Name).AppendLine(".")
                .AppendLine();
        }

        private void AppendCommand(StringBuilder str, Command cmd)
        {
            str.Append($"{App.ExecutableName} {cmd.FullName}");

            foreach (var arg in cmd.Arguments)
            {
                AppendArgument(str, arg);
            }

            foreach (var opt in cmd.Options)
            {
                AppendOption(str, opt);
            }

            str.AppendLine();
        }

        private static void AppendArgument(StringBuilder str, Argument arg)
        {
            str.Append(" ");

            if (arg.Required)
                str.Append("<");
            else
                str.Append("[");

            str.Append(arg.Name);

            if (arg.Required)
                str.Append(">");
            else
                str.Append("]");

            if (arg.Type.IsArray)
                str.Append("...");
        }

        private static void AppendOption(StringBuilder str, Option opt, bool appendDelims = true)
        {
            if (appendDelims)
                str.Append(" [");

            str.Append(opt);

            if (appendDelims)
                str.Append("]");
        }
    }
}
