using System.Text;

namespace Lexer
{
    public abstract record Atom
    {
        protected StringBuilder lexeme;
        public string Code { get; protected set; } = null;
        public string Lexeme => lexeme.ToString();
        public int LineOcurrency = 0;
        public int OriginalLenght = 0;
        public int TruncatedLenght = 0;
        public bool Identifier { get => ReservedTable.GetTokenCode(Lexeme) == null; }


        public Atom(Atom a)
        {
            if (a == null)
                lexeme = new();
            else
                lexeme = new(a.lexeme.ToString());

        }
        public abstract (Atom, Atom) ConsumeChar(char c, FileReader reader);

        protected bool ValidSymbol(char c) => "!=#&();[]{},<>%/*+-".Contains(c);

        public void Truncate()
        {
            OriginalLenght = Lexeme.Length;

            if (OriginalLenght > 30)
            {
                string truncated = Lexeme;
                if (Code == "C06") //float
                {

                    int last = 29;
                    while (!char.IsDigit(Lexeme[last]))
                    {
                        last--;
                    }
                    truncated = Lexeme.Substring(0, last + 1);

                    if (!truncated.Contains('.'))
                    {
                        Code = "C03";
                    }

                }
                else if (Code == "C02") //string
                {
                    truncated = Lexeme.Substring(0, 29) + "\"";
                }
                else
                {
                    truncated = Lexeme.Substring(0, 30);
                }

                lexeme = new(truncated);
            }
            TruncatedLenght = Lexeme.Length;
        }

        public string Print() => $"{LineOcurrency}. <{Lexeme},{Code}>";

    }




}
