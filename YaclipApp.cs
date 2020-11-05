using Yaclip.Builders;
using Yaclip.Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Yaclip
{
    public class YaclipApp
    {
        internal CommandCollection Commands { get; }
        internal string? Name { get; }
        internal string? ExecutableName { get; }

        public static IYaclipAppBuilder New() => new YaclipAppBuilder();

        internal YaclipApp(CommandCollection commands, string? name, string? exeName, bool generateHelp)
        {
            this.Commands = commands;
            this.Name = name ?? Process.GetCurrentProcess().ProcessName;
            this.ExecutableName = exeName ?? Process.GetCurrentProcess().ProcessName;

            if (generateHelp)
                this.Commands.Add(new HelpCommand(this));
        }

        public void Run(string[] args)
        {
            try
            {
                RunInner(args);
            }
            catch (RunException ex) when (!Debugger.IsAttached)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(ex.Message);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        private void RunInner(string[] args)
        {
            var tokens = new Queue<IToken>(Parser.Parse(args));

            Command cmd;

            if (tokens.Count == 0)
            {
                if (Commands.TryGet(out var rootCmd, ""))
                    cmd = rootCmd;
                else if (Commands.TryGet(out var helpCmd, "help"))
                    cmd = helpCmd;
                else
                    return;
            }
            else
            {
                cmd = GetCommand(tokens);
            }

            var optionsObj = cmd.Factory();

            var ctx = new RunContext(optionsObj, tokens, cmd);
            while (tokens.Count > 0)
            {
                ApplyToken(ref ctx);
            }

            int requiredArgCount = cmd.Arguments.Count(o => o.Required);
            if (requiredArgCount > ctx.SetArgumentsCount)
                throw new RunException("Missing required arguments: " + string.Join(", ", cmd.Arguments.Skip(ctx.SetArgumentsCount).Where(o => o.Required).Select(o => o.Name)));

            Environment.ExitCode = cmd.Run(optionsObj);
        }

        private Command GetCommand(Queue<IToken> tokens)
        {
            int i = 0;
            var cmdSoFar = new StringBuilder();

            var commands = new List<Command>(Commands);

            while (commands.Count > 1 && tokens.Count > 0)
            {
                var token = tokens.Dequeue();

                if (!(token is StringToken strToken))
                    throw new RunException($"Expected command name at {token.Position}");

                cmdSoFar.Append(strToken.Content).Append(' ');

                commands.RemoveAll(o => o.Name.Length <= i || o.Name[i] != strToken.Content);
                i++;
            }

            if (commands.Count != 1)
            {
                string msg = $"Unknown command '{cmdSoFar.ToString().TrimEnd()}'. ";

                if (commands.Count > 1)
                    msg += "Did you mean " + string.Join(", ", commands.Select(o => $"'{o.FullName}'")) + "?";

                throw new RunException(msg);
            }

            return commands[0];
        }

        private static void ApplyToken(ref RunContext ctx)
        {
            var token = ctx.Tokens.Peek();

            if (token is IOptionToken opt)
            {
                ctx.Tokens.Dequeue();
                ApplyOption(ref ctx, opt);
            }
            else
            {
                ApplyArgument(ref ctx);
            }
        }

        private static void ApplyArgument(ref RunContext ctx)
        {
            if (ctx.SetArgumentsCount == ctx.Command.Arguments.Length)
                throw new RunException("Argument found when no more were necessary");

            var arg = ctx.Command.Arguments[ctx.SetArgumentsCount];
            ctx.SetArgumentsCount++;

            if (!TryTakeValue(ref ctx, arg.Type, out var value))
                throw new RunException($"Cannot parse value at position {ctx.Tokens.PeekOrDefault()?.Position}");

            ValueSetter.SetValue(ctx.OptionsObj, arg.MemberExpression, value, false);
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

            var propertyType = opt.ValueType;
            bool isList = false;

            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(IList<>))
            {
                propertyType = propertyType.GetGenericArguments()[0];
                isList = true;
            }

            if (!TryTakeValue(ref ctx, propertyType, out var value))
            {
                if (opt.ValueType == typeof(bool))
                    value = true;
                else
                    throw new RunException($"Expected value at position {ctx.Tokens.PeekOrDefault()?.Position}");
            }

            ValueSetter.SetValue(ctx.OptionsObj, opt.MemberExpression, value, isList);
        }

        private static bool TryTakeValue(ref RunContext ctx, Type valueType, [MaybeNullWhen(false)] out object value)
        {
            if (valueType.IsListType(out var itemType))
            {
                var del = (TakeListDelegate)typeof(YaclipApp).GetMethod(nameof(TakeList), BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(itemType).CreateDelegate(typeof(TakeListDelegate));
                value = del(ctx);
                return true;
            }

            if (!(ctx.Tokens.PeekOrDefault() is StringToken strToken))
            {
                value = null;
                return false;
            }

            ctx.Tokens.Dequeue();
            return ValueSetter.TryParseValue(strToken.Content, valueType, out value);
        }

        private delegate object TakeListDelegate(RunContext ctx);
        private static object TakeList<T>(RunContext ctx)
        {
            var list = new List<T>();

            while (ctx.Tokens.PeekOrDefault() is StringToken strToken)
            {
                ctx.Tokens.Dequeue();

                if (!ValueSetter.TryParseValue(strToken.Content, typeof(T), out var obj))
                    throw new RunException($"Cannot parse value into {typeof(T).Name} at position {strToken.Position}");

                list.Add((T)obj);
            }

            return list;
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
