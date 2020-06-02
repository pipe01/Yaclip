using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Yaclip
{
    internal class HelpCommand : Command
    {
        private class HelpOptions
        {
            public string? Command { get; set; }
        }

        private readonly YaclipApp App;

        public HelpCommand(YaclipApp app)
        {
            this.App = app ?? throw new ArgumentNullException(nameof(app));

            this.Name = "help";
            this.Description = "Provides help about the usage of the program and its commands.";
            this.Options = Array.Empty<Option>();
            this.Arguments = new[] { new Argument(typeof(string), Expression.Property(Expression.Variable(typeof(HelpOptions)), nameof(HelpOptions.Command)), false, "command") };
            this.ObjectType = typeof(HelpOptions);
        }

        public override void Run(object obj)
        {
            var opts = (HelpOptions)obj;

            if (opts.Command == null)
            {
                Console.WriteLine(GeneralHelp());
            }
            else
            {
                if (!App.Commands.TryGetValue(opts.Command, out var cmd))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Invalid command '{opts.Command}'");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    return;
                }

                Console.WriteLine(HelpForCommand(cmd));
            }
        }

        private string GeneralHelp()
        {
            var str = new StringBuilder();
            AppendHeader(str);

            str.AppendLine("Usage:");

            foreach (var cmd in App.Commands.Values)
            {
                str.Append("  ");
                AppendCommand(str, cmd);
            }

            return str.ToString();
        }

        private string HelpForCommand(Command cmd)
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

            return str.ToString();
        }

        private void AppendHeader(StringBuilder str)
        {
            str.Append(App.Name).AppendLine(".")
                .AppendLine();
        }

        private void AppendCommand(StringBuilder str, Command cmd)
        {
            str.Append($"{App.ExecutableName} {cmd.Name}");

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
