namespace Lexer
{
    public record AtomNone : Atom
    {
        public AtomNone() : base(null)
        {

        }
        public override (Atom, Atom) ConsumeChar(char c, FileReader reader)
        {
            switch (c)
            {

                case (>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or '_':
                    lexeme.Append(c);
                    return (new AtomIdentifier(this), null);
                case (>= '0' and <= '9'):
                    lexeme.Append(c);
                    return (new AtomInteger(this), null);
                case '\"':
                    lexeme.Append(c);
                    return (new AtomString(this), null);
                case '\'':
                    lexeme.Append(c);
                    return (new AtomCharacter(this), null);

                default:
                    if (ValidSymbol(c))
                    {
                        lexeme.Append(c);
                        return (new AtomSymbol(this), null);
                    }
                    return (this, null);
            }
        }
    }
}