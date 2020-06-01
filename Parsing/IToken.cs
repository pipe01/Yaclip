namespace LogicCommandLineParser.Parsing
{
    internal interface IToken
    {
        int Position { get; }
    }

    internal interface IOptionToken : IToken
    {
    }
}
