namespace Lexer
{
    public class AtomComma : Atom
    {

        public override string Code => "B09";
        public AtomComma(Atom a) : base(a)
        {

        }
        public override (Atom, Atom) ConsumeChar(char c)
        {
            switch (c)
            {
                case ';':
                    lexeme.Append(c);
                    return (this, null);
                default:
                    return (new AtomNone().ConsumeChar(c).Item1, this);
            };
        }
    }

}