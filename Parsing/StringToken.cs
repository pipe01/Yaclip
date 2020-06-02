using System;

namespace Yaclip.Parsing
{
    internal class StringToken : IToken
    {
        public string Content { get; }
        public int Position { get; }

        public StringToken(string content, int position)
        {
            this.Content = content ?? throw new ArgumentNullException(nameof(content));
            this.Position = position;
        }

        public override string ToString() => Content;
    }
}
