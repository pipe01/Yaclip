using LogicCommandLineParser.Builders;
using LogicCommandLineParser.Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LogicCommandLineParser
{
    public class LogicApp
    {
        internal IDictionary<string, Command> Commands { get; } = new Dictionary<string, Command>();
        internal string? Name { get; }
        internal string? ExecutableName { get; }

        public static ILogicAppBuilder New() => new LogicAppBuilder();

        internal LogicApp(IEnumerable<Command> commands, string? name, string? exeName, bool generateHelp)
        {
            this.Commands = commands.ToDictionary(o => o.Name);
            this.Name = name ?? Process.GetCurrentProcess().ProcessName;
            this.ExecutableName = exeName ?? Process.GetCurrentProcess().ProcessName;

            if (generateHelp)
                this.Commands.Add("help", new HelpCommand(this));
        }

        public void Run(string[] args)
        {
            try
            {
                RunInner(args);
            }
            catch (RunException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(ex.Message);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        private void RunInner(string[] args)
        {
            var tokens = new Queue<IToken>(Parser.Parse(args));

            if (tokens.Count == 0)
            {
                if (Commands.ContainsKey("help"))
                    tokens.Enqueue(new StringToken("help", 0));
                else
                    return;
            }

            if (!(tokens.Dequeue() is StringToken cmdToken))
                throw new RunException("Missing command name");

            string cmdName = cmdToken.Content;

            if (!Commands.TryGetValue(cmdName, out var cmd))
                throw new RunException("Unknown command");

            var optionsObj = Activator.CreateInstance(cmd.ObjectType);

            var ctx = new RunContext(optionsObj, tokens, cmd);
            while (tokens.Count > 0)
            {
                ApplyToken(ref ctx, tokens.Dequeue());
            }

            int requiredArgCount = cmd.Arguments.Count(o => o.Required);
            if (requiredArgCount > ctx.SetArgumentsCount)
                throw new RunException("Missing required arguments: " + string.Join(", ", cmd.Arguments.Skip(ctx.SetArgumentsCount).Where(o => o.Required).Select(o => o.Name)));

            cmd.Run(optionsObj);
        }

        private static void ApplyToken(ref RunContext ctx, IToken token)
        {
            if (token is StringToken strToken)
            {
                ApplyArgument(ref ctx, strToken);
            }
            else if (token is IOptionToken opt)
            {
                ApplyOption(ref ctx, opt);
            }
        }

        private static void ApplyArgument(ref RunContext ctx, StringToken token)
        {
            if (ctx.SetArgumentsCount == ctx.Command.Arguments.Length)
                throw new RunException("Argument found when no more were necessary");

            var arg = ctx.Command.Arguments[ctx.SetArgumentsCount];
            ctx.SetArgumentsCount++;

            var value = ValueSetter.ParseValue(token.Content, arg.Type);

            ValueSetter.SetValue(ctx.OptionsObj, arg.MemberExpression, value);
        }

        private static void ApplyOption(ref RunContext ctx, IOptionToken token)
        {
            Option? opt = null;

            if (token is ShortOptionToken shortOpt)
                opt = ctx.Command.Options.SingleOrDefault(o => o.ShortName == shortOpt.Content);
            else if (token is LongOptionToken longOpt)
                opt = ctx.Command.Options.SingleOrDefault(o => o.LongName == longOpt.Content);

            if (opt == null)
                throw new RunException($"Unknown option at position {token.Position}");

            object value;

            var valueToken = ctx.Tokens.PeekOrDefault();
            if (valueToken is StringToken valStrToken)
            {
                ctx.Tokens.Dequeue();
                value = ValueSetter.ParseValue(valStrToken.Content, opt.ValueType);
            }
            else if (opt.ValueType == typeof(bool))
            {
                value = true;
            }
            else
            {
                throw new RunException($"Expected value at position {valueToken.Position}");
            }

            ValueSetter.SetValue(ctx.OptionsObj, opt.MemberExpression, value);
        }

        private ref struct RunContext
        {
            public object OptionsObj { get; }
            public Queue<IToken> Tokens { get; }
            public Command Command { get; }

            public int SetArgumentsCount { get; set; }

            public RunContext(object optionsObj, Queue<IToken> tokens, Command command)
            {
                this.OptionsObj = optionsObj;
                this.Tokens = tokens;
                this.Command = command;
                this.SetArgumentsCount = 0;
            }
        }
    }
}
