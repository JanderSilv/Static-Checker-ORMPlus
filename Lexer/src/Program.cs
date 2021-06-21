using Lexer;
using static System.Console;

string input_path = "test.c";

LexicalAnalizer lex = new LexicalAnalizer(new FileReader(input_path));

Atom token = null;

while ((token = lex.NextToken()) != null)
{
    WriteLine(token.Print());
}
WriteLine();
WriteLine();
WriteLine(SymbolTable.ToString());