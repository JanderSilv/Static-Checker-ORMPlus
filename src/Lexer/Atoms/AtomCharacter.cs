namespace Lexer
{
    public class MiddleCharacter : Atom
    {

        public MiddleCharacter(Atom a) : base(a)
        {

        }
        public override (Atom, Atom) ConsumeChar(char c)
        {
            switch (c)
            {
                case '\'':
                    lexeme.Append(c);
                    return (new AtomNone(), new AtomCharacter(this));
                default:
                    return (new AtomNone().ConsumeChar(c).Item1, null);
            };
        }
    }
    public class AtomCharacter : Atom
    {
        public AtomCharacter(Atom a) : base(a)
        {
            Code = "C05";
        }
        public override (Atom, Atom) ConsumeChar(char c)
        {
            switch (c)
            {
                case (>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or (>= '0' and <= '9') or '_' or ' ' or '$':
                    lexeme.Append(c);
                    return (new MiddleCharacter(this), null);
                default:
                    return (new AtomNone().ConsumeChar(c).Item1, this);
            };
        }
    }
}