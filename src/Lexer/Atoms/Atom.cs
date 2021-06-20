using System.Text;

namespace Lexer
{
    public abstract class Atom
    {
        protected StringBuilder lexeme;
        public string Code { get; protected set; } = null;
        public string Lexeme => lexeme.ToString();


        public Atom(Atom a)
        {
            if (a == null)
                lexeme = new();
            else
                lexeme = new(a.lexeme.ToString());

        }
        public abstract (Atom, Atom) ConsumeChar(char c);

        protected bool ValidSymbol(char c) => "!=#&();[]{},<>%/*+-".Contains(c);

        public override string ToString() => $"<{Lexeme},{Code}>";

    }




}
