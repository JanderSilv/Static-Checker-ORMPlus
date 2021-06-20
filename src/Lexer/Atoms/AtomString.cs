namespace Lexer
{
    public class MiddleString : Atom
    {
        public override string Code => "";
        public MiddleString(Atom a) : base(a)
        {

        }
        public override (Atom, Atom) ConsumeChar(char c)
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
                    return (new AtomNone().ConsumeChar(c).Item1, null);
            };
        }
    }
    public class AtomString : Atom
    {
        public override string Code => "C02";
        public AtomString(Atom a) : base(a)
        {

        }
        public override (Atom, Atom) ConsumeChar(char c)
        {
            switch (c)
            {
                case (>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or (>= '0' and <= '9') or '_' or ' ' or '$':
                    lexeme.Append(c);
                    return (new MiddleString(this), null);
                default:
                    return (new AtomNone().ConsumeChar(c).Item1, this);
            };
        }
    }
}