using System;
using System.Collections.Generic;
using System.Text;

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

    TokenType CheckType(char c, TokenType t) => (c, t) switch
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
        int cc = 0;
        foreach (var s in strings)
        {
            if (s.Length == 0) continue;

            currentToken = TokenType.none;
            buffer.Clear();

            antJ = currentAntJ;
            for (int strIndex = 0; strIndex < s.Length; strIndex++)
            {
                char c = s[strIndex];

                TokenType lastToken = currentToken;
                currentToken = CheckType(c, currentToken);



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

                if (currentToken == TokenType.naoTerminal)
                {
                    if (strIndex + 1 < s.Length)
                    {
                        var aux = new List<char>() { '(', ')', '[', ']', '{', '}', '\"' };
                        if (aux.Contains(s[strIndex + 1]))
                        {
                            currentToken = TokenType.fimNaoTerminal;
                        }
                    }
                    else
                    {
                        currentToken = TokenType.fimNaoTerminal;
                    }
                }

                switch (currentToken)
                {
                    case TokenType.erro:
                        throw new Exception($"char: {c} buffer: {buffer.ToString()} currentToken: {lastToken}");

                    case TokenType.fimNaoTerminal or TokenType.fimTerminal:
                        noScope();
                        break;

                    case TokenType.agrupamento:
                        strIndex = opcional(c, strIndex, s);
                        break;
                    case TokenType.opcional:
                        strIndex = opcional(c, strIndex, s, true);
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

            if (strings.Length > 1 && cc < strings.Length)
            {
                parsed.Add(new No() { value = "", indexStart = -1, indexEnd = currentAntJ, type = NoType.ou });
            }
            cc++;
        }

        return parsed;
    }

    void noScope()
    {
        parsed.Add(new No() { value = buffer.ToString(), indexStart = antJ, indexEnd = j, type = NoType.atomo });
        buffer.Clear();
        antJ = j;
        j++;
        currentToken = TokenType.none;
    }

    int opcional(char c, int strIndex, string str, bool opcional = false)
    {
        int fechamento = IndexFechamento(c, str, strIndex + 1);
        string op = str[strIndex..(fechamento + 1)];

        Leitor leitor = new(op[1..(op.Length - 1)], antJ, this);
        var res = leitor.Run(j + 1, antJ);
        parsed.Add(new No { value = op, indexStart = antJ, indexEnd = j, childs = res, type = opcional ? NoType.colchete : NoType.parenteses });
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
        parsed.Add(new No { value = op, indexStart = antJ, indexEnd = j, childs = res, type = NoType.chave });
        buffer.Clear();
        antJ = j;
        j = leitor.j;
        currentToken = TokenType.none;
        return fechamento;
    }


}