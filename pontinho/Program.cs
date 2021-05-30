﻿using System;
using System.Collections.Generic;
using System.Text;


var txt = "\"a\"\"b\"(\"c\"| A (\"a\"|\"b\"))|[\"a\"|\"b\"]{\"b\"\"c\"|A|B}"; /*"\"a\"\"b\"(\"c\" A)[\"a\"]{ \"b\"\"c\"}";*/

Leitor l = new Leitor(txt);
var res = l.Run();
print(res);


void print(List<No> itens, int pad = 0)
{
    foreach (var item in itens)
    {
        var str = $"{item.indexStart}->{item.value}->{item.indexEnd}";
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
    erro,
    espaco
}

public struct No
{
    public string value;
    public int indexEnd;
    public int indexStart;

    public List<No> childs;
};

public class Leitor
{
    public TokenType currentToken = TokenType.none;
    public int j;
    public readonly int sj;
    public Leitor parent;
    public string content;
    StringBuilder buffer = new StringBuilder();
    List<No> parsed = new List<No>();
    int antJ;

    public Leitor(string str, int sj = 1, Leitor parent = null)
    {
        this.sj = sj;
        this.parent = parent;
        this.content = str;
        j = sj;
    }

    TokenType CheckType(char c) => (c, currentToken) switch
    {
        ('"', TokenType.none) => TokenType.terminal,
        ('"', TokenType.terminal) => TokenType.fimTerminal,
        (_, TokenType.terminal) => TokenType.terminal,
        (' ', TokenType.none) => TokenType.espaco,


        ('(', TokenType.none) => TokenType.agrupamento,
        (')', TokenType.agrupamento) => TokenType.none,

        ('[', TokenType.none) => TokenType.opcional,
        (']', TokenType.opcional) => TokenType.none,

        ('{', TokenType.none) => TokenType.recursividade,
        ('}', TokenType.recursividade) => TokenType.none,



        ((>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or (>= '0' and <= '9'), TokenType.none) => TokenType.naoTerminal,
        ((>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or (>= '0' and <= '9'), TokenType.naoTerminal) => TokenType.naoTerminal,
        (_, TokenType.naoTerminal) => TokenType.fimNaoTerminal,

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

    string[] DivideAlternativas(string str)
    {
        List<string> alternativas = new();
        Stack<char> stack = new Stack<char>();
        StringBuilder stb = new StringBuilder();

        foreach (var c in str)
        {
            switch (c)
            {
                case '(' or '[' or '{':
                    stack.Push(c);
                    break;
                case ')' or ']' or '}':
                    char peek = stack.Peek();
                    switch (peek, c)
                    {
                        case ('(', ')') or ('[', ']') or ('{', '}'):
                            stack.Pop();
                            break;
                        default:
                            throw new Exception("Fechamento nao esperado");
                    }
                    break;
                case '|':
                    if (stack.Count == 0)
                    {
                        alternativas.Add(stb.ToString());
                        stb.Clear();
                        continue;
                    }
                    break;
            }

            stb.Append(c);
        }

        alternativas.Add(stb.ToString());
        return alternativas.ToArray();
    }
    public List<No> Run(int? jp = null, int? ajp = null)
    {


        antJ = ajp ?? 0;

        if (jp != null)
        {
            j = jp.Value;
        }


        string[] strings = DivideAlternativas(content);

        int currentAntJ = antJ;
        foreach (var s in strings)
        {
            Console.WriteLine($"Alternative: {s}");
            if (s.Length == 0) continue;
            currentToken = TokenType.none;
            buffer.Clear();

            antJ = currentAntJ;
            for (int strIndex = 0; strIndex < s.Length; strIndex++)
            {
                char c = s[strIndex];
                TokenType lastToken = currentToken;
                currentToken = CheckType(c);
                if (currentToken == TokenType.none)
                {
                    buffer.Clear();
                    continue;
                }
                if (currentToken == TokenType.espaco)
                {
                    currentToken = TokenType.none;
                    continue;
                }

                buffer.Append(c);

                switch (currentToken)
                {
                    case TokenType.erro:
                        throw new Exception($"char: {c} buffer: {buffer.ToString()} currentToken: {lastToken}");

                    case TokenType.fimNaoTerminal or TokenType.fimTerminal:
                        noScope();
                        break;

                    case TokenType.agrupamento or TokenType.opcional:
                        strIndex = opcional(c, strIndex, s);
                        break;

                    case TokenType.recursividade:
                        strIndex = recursao(c, strIndex, s);
                        break;
                }
            }

            switch (currentToken)
            {
                case TokenType.fimNaoTerminal or TokenType.fimTerminal or TokenType.terminal or TokenType.naoTerminal:
                    noScope();
                    break;
            }
        }

        return parsed;
    }

    void noScope()
    {
        parsed.Add(new No() { value = buffer.ToString(), indexStart = antJ, indexEnd = j });
        buffer.Clear();
        antJ = j;
        j++;
        currentToken = TokenType.none;
    }

    int opcional(char c, int strIndex, string str)
    {
        int fechamento = IndexFechamento(c, str, strIndex + 1);
        string op = str[strIndex..(fechamento + 1)];

        Leitor leitor = new(op[1..(op.Length - 1)], antJ, this);
        var res = leitor.Run(j + 1, antJ);
        parsed.Add(new No { value = op, indexStart = antJ, indexEnd = j, childs = res });
        buffer.Clear();
        antJ = j;
        j = leitor.j;
        currentToken = TokenType.none;

        return fechamento;
    }

    int recursao(char c, int strIndex, string str)
    {
        int fechamento = IndexFechamento(c, str, strIndex + 1);
        string op = str[strIndex..(fechamento + 1)];

        Leitor leitor = new(op[1..(op.Length - 1)], j, this);

        var res = leitor.Run(j + 1, j);
        parsed.Add(new No { value = op, indexStart = antJ, indexEnd = j, childs = res });
        buffer.Clear();
        antJ = j;
        j = leitor.j;
        currentToken = TokenType.none;
        return fechamento;
    }


}