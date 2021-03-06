namespace Lexer
{
    public record AtomIdentifier : Atom
    {
        public AtomIdentifier(Atom a) : base(a)
        {
            Code = "C04";
        }
        public override (Atom, Atom) ConsumeChar(char c, FileReader reader)
        {
            switch (c)
            {
                case (>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or (>= '0' and <= '9') or '_':
                    lexeme.Append(c);
                    return (this, null);
                default:
                    string cod = ReservedTable.GetTokenCode(this.Lexeme);
                    if (cod != null) Code = cod;
                    return (new AtomNone().ConsumeChar(c, reader).Item1, this);
            };
        }
    }

}