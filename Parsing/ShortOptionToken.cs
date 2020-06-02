namespace Yaclip.Parsing
{
    internal class ShortOptionToken : IOptionToken
    {
        public char Content { get; }
        public int Position { get; }

        public ShortOptionToken(char content, int position)
        {
            this.Content = content;
            this.Position = position;
        }

        public override string ToString() => $"-{Content}";
    }
}
