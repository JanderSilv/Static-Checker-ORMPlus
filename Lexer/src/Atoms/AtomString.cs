namespace Lexer
{
    public record MiddleString : Atom
    {
        public MiddleString(Atom a) : base(a)
        {
            Code = null;
        }
        public override (Atom, Atom) ConsumeChar(char c, FileReader reader)
        {
            switch (c)
            {
                case (>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or (>= '0' and <= '9') or '_' or ' ' or '$':
                    lexeme.Append(c);
                    return (this, null);
                case '\"':
                    lexeme.Append(c);
                    return (new AtomNone(), new AtomString(this));
                default:
                    return (new AtomNone().ConsumeChar(c, reader).Item1, null);
            };
        }
    }
    public record AtomString : Atom
    {
        public AtomString(Atom a) : base(a)
        {
            Code = "C02";
        }
        public override (Atom, Atom) ConsumeChar(char c, FileReader reader)
        {
            switch (c)
            {
                case (>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or (>= '0' and <= '9') or '_' or ' ' or '$':
                    lexeme.Append(c);
                    return (new MiddleString(this), null);
                default:
                    return (new AtomNone().ConsumeChar(c, reader).Item1, this);
            };
        }
    }
}