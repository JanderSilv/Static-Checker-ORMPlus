namespace Lexer
{
    public record AtomComment : Atom
    {
        private bool multline = false;
        public AtomComment(Atom a) : base(a)
        {
            if (Lexeme == "/*") multline = true;
            lexeme.Clear();

        }
        public override (Atom, Atom) ConsumeChar(char c, FileReader reader)
        {
            lexeme.Append(c);

            if (multline)
            {
                if (Lexeme.EndsWith("*/")) return (new AtomNone(), null);
                return (this, null);
            }
            else
            {
                if (c == '\n') return (new AtomNone(), null);
                return (this, null);
            }

        }
    }

}