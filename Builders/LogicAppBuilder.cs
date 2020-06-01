using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicCommandLineParser.Builders
{
    public interface ILogicAppBuilder
    {
        ILogicAppBuilder Command<T>(string name, Action<ICommandBuilder<T>> builder) where T : new();
        ILogicAppBuilder Name(string name);
        ILogicAppBuilder ExecutableName(string name);
        ILogicAppBuilder GenerateHelpCommand(bool generate);

        LogicApp Build();
    }

    internal class LogicAppBuilder : ILogicAppBuilder
    {
        private readonly IList<Command> Commands = new List<Command>();
        private string? Name, ExecutableName;
        private bool GenerateHelp = true;

        ILogicAppBuilder ILogicAppBuilder.Command<T>(string name, Action<ICommandBuilder<T>> builder)
        {
            var b = new CommandBuilder<T>(name);
            builder(b);

            var cmd = b.Build();

            if (Commands.Any(o => o.Name == cmd.Name))
                throw new BuilderException($"Duplicate command name '{cmd.Name}'");

            Commands.Add(cmd);
            return this;
        }

        ILogicAppBuilder ILogicAppBuilder.Name(string name)
        {
            Name = name;
            return this;
        }

        ILogicAppBuilder ILogicAppBuilder.ExecutableName(string name)
        {
            ExecutableName = name;
            return this;
        }

        ILogicAppBuilder ILogicAppBuilder.GenerateHelpCommand(bool generate)
        {
            GenerateHelp = generate;
            return this;
        }

        public LogicApp Build()
        {
            return new LogicApp(Commands, Name, ExecutableName, GenerateHelp);
        }
    }
}
