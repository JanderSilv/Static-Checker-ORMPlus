namespace Lexer
{
    public class AtomExponent : Atom
    {
        public override string Code => "C06";
        private bool signed = false;
        public AtomExponent(Atom a) : base(a)
        {

        }

        public override (Atom, Atom) ConsumeChar(char c)
        {
            switch (c)
            {
                case '+' or '-':
                    if (!signed)
                    {
                        lexeme.Append(c);
                        signed = true;
                        return (this, null);
                    }
                    else
                    {
                        return (new AtomNone().ConsumeChar(c).Item1, this);
                    }

                case (>= '0' and <= '9'):
                    signed = true;
                    lexeme.Append(c);
                    return (this, null);
                default:
                    string cod = ReservedTable.GetTokenCode(this.Lexeme);
                    return (new AtomNone().ConsumeChar(c).Item1, this);
            };
        }
    }
    public class AtomFloat : Atom
    {

        public override string Code => "C06";
        public AtomFloat(Atom a) : base(a)
        {

        }
        public override (Atom, Atom) ConsumeChar(char c)
        {
            switch (c)
            {
                case (>= '0' and <= '9'):
                    lexeme.Append(c);
                    return (this, null);
                case 'e':
                    lexeme.Append(c);
                    return (new AtomExponent(this), null);
                default:
                    string cod = ReservedTable.GetTokenCode(this.Lexeme);
                    return (new AtomNone().ConsumeChar(c).Item1, this);
            };
        }
    }
}