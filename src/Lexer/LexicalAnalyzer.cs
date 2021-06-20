using static System.Console;

namespace Lexer
{
    public class LexicalAnalizer
    {
        FileReader source;

        public LexicalAnalizer(FileReader Source)
        {
            source = Source;
        }

        public Atom NextToken()
        {
            char? c = null;
            Atom current = new AtomNone();
            Atom formed = null;

            while (formed == null)
            {
                c = source.GetNext();

                if (c == null)
                {
                    (current, formed) = current.ConsumeChar(' ');
                    break;
                }
                else
                    (current, formed) = current.ConsumeChar(c.Value);

            }

            return formed;

        }
    }
}