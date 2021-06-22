using static System.Console;

namespace Lexer
{
    public class LexicalAnalizer
    {
        private readonly FileReader source;
        private Atom current = new AtomNone();
        public LexicalAnalizer(FileReader Source)
        {
            source = Source;
        }

        public Atom NextToken()
        {
            char? c = null;

            Atom formed = null;
            int line = 0;
            while (formed == null)
            {
                line = source.CurrentLine;
                c = source.GetNext();

                if (c == null)
                {
                    (current, formed) = current.ConsumeChar(' ', source);

                    break;
                }
                else
                {
                    (current, formed) = current.ConsumeChar(c.Value, source);

                }

            }
            if (formed != null)
            {
                formed.LineOcurrency = line;
                formed.Truncate();
                int? exist = SymbolTable.ExistSymbol(formed.Lexeme);
                if (exist != null)
                {
                    Symbol s = SymbolTable.GetSymbol(exist.Value);
                    formed.Code = s.Code;
                }

                SymbolTable.AddSymbol(formed);
            }

            return formed;

        }
    }
}