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

        public void NextToken()
        {
            char? c;
            Atom current = new AtomNone();
            Atom formed = null;

            while ((c = source.GetNext()) != null)
            {
                (current, formed) = current.ConsumeChar(c.Value);
                if (formed != null) WriteLine(formed);
            }
        }
    }
}