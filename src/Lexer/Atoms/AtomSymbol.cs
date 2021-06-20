namespace Lexer
{
    public class AtomSymbol : Atom
    {
        public AtomSymbol(Atom a) : base(a)
        {

        }
        public override (Atom, Atom) ConsumeChar(char c)
        {
            if (ValidSymbol(c))
            {
                if (Lexeme + c == "//" || Lexeme + c == "/*")
                {
                    lexeme.Append(c);
                    return (new AtomComment(this), null);
                }
                if (ReservedTable.GetTokenCode(Lexeme + c) != null)
                {
                    lexeme.Append(c);
                    return (this, null);
                }
            }

            Code = ReservedTable.GetTokenCode(this.Lexeme);
            return (new AtomNone().ConsumeChar(c).Item1, this);


        }
    }

}