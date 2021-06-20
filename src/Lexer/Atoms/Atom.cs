using System.Text;

namespace Lexer
{
    public abstract class Atom
    {
        protected StringBuilder lexeme;
        public virtual string Code => null;
        public string Lexeme => lexeme.ToString();


        public Atom(Atom a)
        {
            if (a == null)
                lexeme = new();
            else
                this.lexeme = a.lexeme;
        }
        public abstract (Atom, Atom) ConsumeChar(char c);

        public override string ToString() => $"<{Lexeme},{Code}>";

    }




}
