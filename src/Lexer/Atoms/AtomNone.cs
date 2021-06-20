namespace Lexer
{
    public class AtomNone : Atom
    {
        public override string Code => null;

        public AtomNone() : base(null)
        {

        }
        public override (Atom, Atom) ConsumeChar(char c)
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

                default:
                    if (char.IsSymbol(c) || char.IsPunctuation(c))
                    {
                        lexeme.Append(c);
                        return (new AtomSymbol(this), null);
                    }
                    return (this, null);
            }
        }
    }
}