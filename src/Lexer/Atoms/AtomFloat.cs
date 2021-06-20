using System.Linq;

namespace Lexer
{
    public record AtomExponent : Atom
    {

        private bool signed = false;
        public AtomExponent(Atom a) : base(a)
        {
            Code = "C06";
        }

        public override (Atom, Atom) ConsumeChar(char c, FileReader reader)
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
                        return (new AtomNone().ConsumeChar(c, reader).Item1, this);
                    }

                case (>= '0' and <= '9'):
                    signed = true;
                    lexeme.Append(c);
                    return (this, null);
                default:
                    string cod = ReservedTable.GetTokenCode(this.Lexeme);
                    return (new AtomNone().ConsumeChar(c, reader).Item1, this);
            };
        }
    }
    public record AtomFloat : Atom
    {
        bool fr = true;
        public AtomFloat(Atom a) : base(a)
        {
            Code = "C06";
        }
        public override (Atom, Atom) ConsumeChar(char c, FileReader reader)
        {
            if (fr && !char.IsDigit(c))
            {
                reader.Rev(2);
                lexeme.Remove(lexeme.Length - 1, 1);
                return (new AtomNone(), new AtomInteger(this));
            }
            fr = false;
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
                    return (new AtomNone().ConsumeChar(c, reader).Item1, this);
            };
        }
    }
}