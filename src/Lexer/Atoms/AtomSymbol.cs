namespace Lexer
{
    public class AtomSymbol : Atom
    {
        string code = "";
        public override string Code => code;
        public AtomSymbol(Atom a) : base(a)
        {

        }
        public override (Atom, Atom) ConsumeChar(char c)
        {
            if (char.IsSymbol(c))
            {
                lexeme.Append(c);
                return (this, null);
            }
            else
            {
                string cod = ReservedTable.GetTokenCode(this.Lexeme);
                if (cod != null) code = cod;
                return (new AtomNone().ConsumeChar(c).Item1, this);
            }
        }
    }

}