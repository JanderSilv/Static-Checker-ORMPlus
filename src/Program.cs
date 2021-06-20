using System;
using Lexer;

string input_path = "test.c";

LexicalAnalizer lex = new LexicalAnalizer(new FileReader(input_path));

Atom token = null;

while ((token = lex.NextToken()) != null)
{
    Console.WriteLine(token);
}