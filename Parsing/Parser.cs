using System.Collections.Generic;
using System.Linq;

namespace LogicCommandLineParser.Parsing
{
    internal static class Parser
    {
        private static readonly List<IToken> Tokens = new List<IToken>();

        public static IReadOnlyList<IToken> Parse(string[] args)
        {
            Tokens.Clear();

            int pos = 0;
            foreach (var item in args)
            {
                ParseString(item, pos);

                pos += item.Length + 1; // Add one to account for spaces between items
            }

            return Tokens;
        }

        private static void ParseString(string str, int pos)
        {
            if (str.Length >= 1 && str[0] == '-')
            {
                if (str.Length > 2 && str[1] == '-')
                {
                    if (str.Contains('='))
                    {
                        int equalsIndex = str.IndexOf('=');

                        Tokens.Add(new LongOptionToken(str.Substring(2, equalsIndex - 2), pos));
                        Tokens.Add(new StringToken(str.Substring(equalsIndex + 1), pos + equalsIndex));
                    }
                    else
                    {
                        Tokens.Add(new LongOptionToken(str.Substring(2), pos));
                    }
                }
                else
                {
                    for (int i = 1; i < str.Length; i++)
                    {
                        Tokens.Add(new ShortOptionToken(str[i], pos + i));
                    }
                }
            }
            else
            {
                Tokens.Add(new StringToken(str, pos));
            }
        }
    }
}
