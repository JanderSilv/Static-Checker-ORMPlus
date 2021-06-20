namespace Lexer
{
    public class Symbol
    {
        int TableEntry;
        string Code;
        string Type;
        string Lexeme;
        int[] FirstApearences = new int[5];
        int OriginalLen;
        int TruncatedLen;

        public Symbol(Atom a)
        {

        }
    }
    public static class SymbolTable
    {

    }
}