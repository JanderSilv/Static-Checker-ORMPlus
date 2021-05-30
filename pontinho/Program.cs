using System;
using System.Collections.Generic;
using System.Text;


var txt = "\"a\"\"b\"((\"c\" A))[\"a\"]{ \"b\"\"c\"}";

Leitor l = new Leitor(txt, 0);
var res = l.Run();
print(res);


void print(List<No> itens, int pad = 0)
{
    foreach (var item in itens)
    {
        var str = $"{item.value} = {item.index}";
        str = str.PadLeft(pad);
        Console.WriteLine(str);
        if (item.childs != null) print(item.childs, pad + 1);
    }
}

public enum TokenType
{
    none,

    naoTerminal,
    fimNaoTerminal,

    terminal,
    fimTerminal,

    agrupamento,

    opcional,

    recursividade,
    erro
}

public struct No
{
    public string value;
    public int index;

    public List<No> childs;
};

public class Leitor
{
    public TokenType currentToken = TokenType.none;
    public int j = 0;
    public readonly int sj;
    public Leitor parent;
    public string str;

    public Leitor(string str, int sj = 0, Leitor parent = null)
    {
        Console.WriteLine($"Reading: {str}");
        this.sj = sj;
        this.parent = parent;
        j = sj;
        this.str = str;
    }

    TokenType CheckType(char c) => (c, currentToken) switch
    {
        ('"', TokenType.none) => TokenType.terminal,
        ('"', TokenType.terminal) => TokenType.fimTerminal,
        (_, TokenType.terminal) => TokenType.terminal,
        (' ', _) => TokenType.none,

        ('(', TokenType.none) => TokenType.agrupamento,
        (')', TokenType.agrupamento) => TokenType.none,

        ('[', TokenType.none) => TokenType.opcional,
        (']', TokenType.opcional) => TokenType.none,

        ('{', TokenType.none) => TokenType.recursividade,
        ('}', TokenType.recursividade) => TokenType.none,



        ('"' or '(' or ')' or '[' or ']' or '{' or '}' or ' ', TokenType.naoTerminal) => TokenType.fimNaoTerminal,
        ((>= 'a' and <= 'z') or (>= 'A' or <= 'Z') or (>= '0' or <= '9'), TokenType.none) => TokenType.naoTerminal,
        ((>= 'a' and <= 'z') or (>= 'A' or <= 'Z') or (>= '0' or <= '9'), TokenType.naoTerminal) => TokenType.naoTerminal,


        (_, _) => TokenType.erro
    };

    int IndexFechamento(char abertura, string str, int start)
    {
        char fechamento = abertura switch
        {
            '(' => ')',
            '{' => '}',
            '[' => ']',
            _ => throw new Exception("Caracter passado não delimita escopo")
        };

        Stack<char> stack = new Stack<char>();
        stack.Push(abertura);

        for (int i = start; i < str.Length; i++)
        {
            switch (str[i])
            {
                case '(' or '[' or '{':
                    stack.Push(str[i]);
                    break;
                case ')' or ']' or '}':
                    char peek = stack.Peek();
                    switch (peek, str[i])
                    {
                        case ('(', ')') or ('[', ']') or ('{', '}'):
                            stack.Pop();
                            break;
                        default:
                            throw new Exception("Fechamento nao esperado");
                    }
                    break;
            }

            if (stack.Count == 0)
            {
                return i;
            }

        }

        return -1;
    }

    public List<No> Run(int? jp = null)
    {
        currentToken = TokenType.none;
        StringBuilder buffer = new StringBuilder();
        List<No> parsed = new List<No>();
        if (jp != null) j = jp.Value;
        int antJ = j;

        void noScope()
        {
            j++;
            parsed.Add(new No() { value = buffer.ToString(), index = j });
            buffer.Clear();
            currentToken = TokenType.none;
            antJ = j;
        }

        int opcional(char c, int strIndex)
        {
            int fechamento = IndexFechamento(c, str, strIndex + 1);
            Console.WriteLine($"ini {strIndex} end: {fechamento}");
            string op = str[strIndex..(fechamento + 1)];


            Leitor leitor = new(op[1..(op.Length - 1)], antJ, this);
            antJ = j + 1;
            var res = leitor.Run(j + 1);
            parsed.Add(new No { value = op, index = antJ, childs = res });
            buffer.Clear();
            j = leitor.j;
            currentToken = TokenType.none;

            return fechamento;
        }

        int recursao(char c, int strIndex)
        {
            int fechamento = IndexFechamento(c, str, strIndex + 1);
            string op = str[strIndex..(fechamento + 1)];

            Leitor leitor = new(op[1..(op.Length - 1)], j, this);

            var res = leitor.Run(j);
            antJ = j;
            parsed.Add(new No { value = op, index = j, childs = res });
            buffer.Clear();
            j = leitor.j;
            currentToken = TokenType.none;
            return fechamento;
        }

        for (int strIndex = 0; strIndex < str.Length; strIndex++)
        {
            char c = str[strIndex];
            TokenType lastToken = currentToken;
            currentToken = CheckType(c);
            buffer.Append(c);

            switch (currentToken)
            {
                case TokenType.erro:
                    throw new Exception($"char: {c} buffer: {buffer.ToString()} currentToken: {lastToken}");

                case TokenType.fimNaoTerminal or TokenType.fimTerminal:
                    noScope();
                    break;

                case TokenType.agrupamento or TokenType.opcional:
                    strIndex = opcional(c, strIndex);
                    break;

                case TokenType.recursividade:
                    strIndex = recursao(c, strIndex);
                    break;
            }
        }

        return parsed;
    }

}