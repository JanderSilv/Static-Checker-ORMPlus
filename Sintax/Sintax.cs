using System;
using Lexer;
using System.Collections.Generic;
using System.IO;

public class Sintax
{
    public Sintax(string filepath)
    {
        if (!filepath.Contains(".201"))
        {
            filepath += ".201";
        }

        string filename = Path.GetFileNameWithoutExtension(filepath).ToString();

        SymbolTable.Init();
        LexicalAnalizer lex = new LexicalAnalizer(new FileReader(filepath));
        Atom currentAtom = null;
        StateMachine machine = new ProgramMachine();
        int? res = null;

        void HandleIdentifiers()
        {
            if (currentAtom.Code == "C04")
            {
                Atom next = lex.NextToken();
                if (next != null)
                {
                    if (next.Code != "B05")
                    {
                        currentAtom.Code = "C01";
                        int? index = SymbolTable.ExistSymbol(currentAtom.Lexeme);
                        Symbol symbol = SymbolTable.GetSymbol(index.Value);
                        symbol.UpdateCode("C01");
                        SymbolTable.UpdateSymbol(symbol);
                    }
                    res = machine.NextState(currentAtom.Code);
                    if (res == null && !machine.AceitableState)
                    {
                        Console.WriteLine("Cadeia invalida");
                    }
                    currentAtom = next;
                }
                else
                {
                    Console.WriteLine("Cadeia invalida");
                }
            }
        }

        while ((currentAtom = lex.NextToken()) != null)
        {
            HandleIdentifiers();
            res = machine.NextState(currentAtom.Code);

            if (res == null && !machine.AceitableState)
            {
                Console.WriteLine("Cadeia invalida");
            }
            else if (machine.AceitableState)
            {
                Console.WriteLine("Cadeia valida");
            }
        };

        string dir = Path.GetDirectoryName(Path.GetFullPath(filepath));
        File.WriteAllText(dir + "/" + filename + ".LEX", SymbolTable.GetLex());
        File.WriteAllText(dir + "/" + filename + ".TAB", SymbolTable.GetTab());
        Console.WriteLine("Files saved on dir: " + dir);
    }
}