namespace Lexer
{


    public record AtomInteger : Atom
    {

        public AtomInteger(Atom a) : base(a)
        {
            Code = "C03";
        }
        public override (Atom, Atom) ConsumeChar(char c, FileReader reader)
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
                    return (new AtomNone().ConsumeChar(c, reader).Item1, this);
            };
        }
    }

}