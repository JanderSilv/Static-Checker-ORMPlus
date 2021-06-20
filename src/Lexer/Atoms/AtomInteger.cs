namespace Lexer
{


    public class AtomInteger : Atom
    {
        public override string Code => "C03";
        public AtomInteger(Atom a) : base(a)
        {

        }
        public override (Atom, Atom) ConsumeChar(char c)
        {
            switch (c)
            {
                case (>= '0' and <= '9'):
                    lexeme.Append(c);
                    return (this, null);
                case '.':
                    lexeme.Append(c);
                    return (new AtomFloat(this), null);
                default:
                    return (new AtomNone().ConsumeChar(c).Item1, this);
            };
        }
    }

}