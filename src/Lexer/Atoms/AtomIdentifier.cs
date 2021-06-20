namespace Lexer
{
    public class AtomIdentifier : Atom
    {
        public AtomIdentifier(Atom a) : base(a)
        {
            Code = "C01";
        }
        public override (Atom, Atom) ConsumeChar(char c)
        {
            switch (c)
            {
                case (>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or (>= '0' and <= '9') or '_':
                    lexeme.Append(c);
                    return (this, null);
                default:
                    string cod = ReservedTable.GetTokenCode(this.Lexeme);
                    if (cod != null) Code = cod;
                    return (new AtomNone().ConsumeChar(c).Item1, this);
            };
        }
    }

}