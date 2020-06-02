using System;

namespace Yaclip.Parsing
{
    internal class LongOptionToken : IOptionToken
    {
        public string Content { get; }
        public int Position { get; }

        public LongOptionToken(string content, int position)
        {
            this.Content = content ?? throw new ArgumentNullException(nameof(content));
            this.Position = position;
        }

        public override string ToString() => $"--{Content}";
    }
}
