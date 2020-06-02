using System;
using System.Collections.Generic;
using System.Linq;

namespace Yaclip.Builders
{
    public interface IYaclipAppBuilder
    {
        IYaclipAppBuilder Command<T>(string name, Action<ICommandBuilder<T>> builder) where T : new();
        IYaclipAppBuilder Name(string name);
        IYaclipAppBuilder ExecutableName(string name);
        IYaclipAppBuilder GenerateHelpCommand(bool generate);

        YaclipApp Build();
    }

    internal class YaclipAppBuilder : IYaclipAppBuilder
    {
        private readonly IList<Command> Commands = new List<Command>();
        private string? Name, ExecutableName;
        private bool GenerateHelp = true;

        IYaclipAppBuilder IYaclipAppBuilder.Command<T>(string name, Action<ICommandBuilder<T>> builder)
        {
            var b = new CommandBuilder<T>(name);
            builder(b);

            var cmd = b.Build();

            if (Commands.Any(o => o.Name == cmd.Name))
                throw new BuilderException($"Duplicate command name '{cmd.Name}'");

            Commands.Add(cmd);
            return this;
        }

        IYaclipAppBuilder IYaclipAppBuilder.Name(string name)
        {
            Name = name;
            return this;
        }

        IYaclipAppBuilder IYaclipAppBuilder.ExecutableName(string name)
        {
            ExecutableName = name;
            return this;
        }

        IYaclipAppBuilder IYaclipAppBuilder.GenerateHelpCommand(bool generate)
        {
            GenerateHelp = generate;
            return this;
        }

        public YaclipApp Build()
        {
            return new YaclipApp(Commands, Name, ExecutableName, GenerateHelp);
        }
    }
}
