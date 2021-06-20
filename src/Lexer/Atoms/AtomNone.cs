namespace Lexer
{
    public class AtomNone : Atom
    {
        public override string Code => null;

        public AtomNone() : base(null)
        {

        }
        public override (Atom, Atom) ConsumeChar(char c)
        {
            switch (c)
            {
                case (>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or '_':
                    lexeme.Append(c);
                    return (new AtomIdentifier(this), null);
                default:
                    return (this, null);
            };
        }
    }
}