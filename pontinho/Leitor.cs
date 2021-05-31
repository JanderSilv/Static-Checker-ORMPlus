using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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

        (not '"', TokenType.terminal) => TokenType.terminal,

        ('(', not TokenType.terminal) => TokenType.agrupamento,
        (')', TokenType.agrupamento) => TokenType.fechamento,
        (')', not TokenType.agrupamento) => throw new Exception($"Fechamento sem abertura para ).\n({t},{buffer.ToString()} << {c})-> ?"),

        ('[', not TokenType.terminal) => TokenType.opcional,
        (']', TokenType.opcional) => TokenType.fechamento,
        (']', not TokenType.opcional) => throw new Exception($"Fechamento sem abertura para ].\n({t},{buffer.ToString()} << {c})-> ?"),

        ('{', not TokenType.terminal) => TokenType.recursividade,
        ('}', TokenType.recursividade) => TokenType.fechamento,
        ('}', not TokenType.recursividade) => throw new Exception($"Fechamento sem abertura para }}.\n({t},{buffer.ToString()} << {c})-> ?"),

        ('"', not TokenType.terminal) => TokenType.terminal,

        ('"', TokenType.terminal) => TokenType.atomoLido,


        (' ', not TokenType.terminal) => TokenType.none,

        (_, not TokenType.terminal) => TokenType.naoTerminal,

        //(_, _) => throw new Exception($"Transicao não definida: ({t},{buffer.ToString()} + {c})-> ?"),
    };

    int IndexFechamento(char abertura, string str)
    {
        char fechamento = abertura switch
        {
            '(' => ')',
            '{' => '}',
            '[' => ']',
            _ => throw new Exception("Caracter passado não delimita escopo")
        };

        Stack<char> stack = new Stack<char>();
        StringBuilder readed = new();

        stack.Push(abertura);

        TokenType token = TokenType.none;
        readed.Append(abertura);
        for (int i = 1; i < str.Length; i++)
        {
            readed.Append(str[i]);
            if (token == TokenType.terminal)
            {
                if (str[i] == '\"')
                {
                    token = TokenType.none;
                }
                continue;
            }

            if (token == TokenType.none)
            {
                if (str[i] == '\"')
                {
                    token = TokenType.terminal;
                    continue;
                }
            }

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
                            throw new Exception($"Fechamento nao esperado\nPilha final: {stackToString()}\nPassado: {str}\nLidos: {readed.ToString()}");
                    }
                    break;
            }

            if (stack.Count == 0)
            {
                return i;
            }

        }

        string stackToString()
        {
            StringBuilder sb = new();
            foreach (var item in stack)
            {
                sb.Append(item);
                sb.Append(' ');
            }
            return sb.ToString();
        }
        throw new Exception($"Fechamento não encontrado para {abertura}\nPilha final: {stackToString()}\nPassado: {str}\nLidos: {readed.ToString()}");
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

    public List<No> Run(int? j = null, int? aj = null)
    {
        string[] strings = DivideAlternativas(content);
        antJ = aj ?? 0;
        this.j = j ?? this.j;

        int i = 0;
        foreach (var s in strings)
        {
            if (s.Length != 0)
            {
                RunString(s, antJ, i, strings.Length);
            }
            i++;
        }

        return parsed;
    }

    public void RunString(string s, int currentAntJ, int index, int countStrings)
    {
        currentToken = TokenType.none;
        buffer.Clear();
        antJ = currentAntJ;
        TokenType lastToken;

        for (int strIndex = 0; strIndex < s.Length; strIndex++)
        {
            char c = s[strIndex];

            lastToken = currentToken;

            currentToken = CheckType(c, currentToken);

            if (lastToken == TokenType.naoTerminal && currentToken != TokenType.naoTerminal)
            {
                parsed.Add(new No() { value = buffer.ToString(), indexStart = antJ, indexEnd = j, type = NoType.atomo });
                buffer.Clear();
                antJ = j;
                j++;

            }

            buffer.Append(c);


            switch (currentToken)
            {
                case TokenType.none:
                    buffer.Clear();
                    break;

                case TokenType.fechamento:
                    buffer.Clear();
                    currentToken = TokenType.none;
                    break;

                case TokenType.atomoLido:
                    Atomo();
                    currentToken = TokenType.none;
                    break;

                case TokenType.agrupamento:
                    strIndex = OpcionalOuAgrupamento(c, s, strIndex);
                    break;

                case TokenType.opcional:
                    strIndex = OpcionalOuAgrupamento(c, s, strIndex, true);
                    break;

                case TokenType.recursividade:
                    strIndex = Recursao(c, s, strIndex);

                    break;
            }

        }

        switch (currentToken)
        {
            case TokenType.atomoLido or TokenType.terminal or TokenType.naoTerminal:
                Atomo();
                break;
        }

        if (countStrings > 1 && index < countStrings)
        {
            parsed.Add(new No() { value = "", indexStart = -1, indexEnd = currentAntJ, type = NoType.ou });
        }

    }

    void Atomo()
    {
        parsed.Add(new No() { value = buffer.ToString(), indexStart = antJ, indexEnd = j, type = NoType.atomo });
        buffer.Clear();
        antJ = j;
        j++;

    }

    int OpcionalOuAgrupamento(char c, string str, int strIndex, bool opcional = false)
    {
        int end = strIndex + IndexFechamento(c, str[strIndex..]);

        try
        {
            str = str[(strIndex + 1)..end];
        }
        catch (System.Exception e)
        {
            Console.WriteLine($"Try to substring at {(strIndex + 1)} - {end}");
            throw e;
        }


        Leitor leitor = new(str, antJ, this);
        var res = leitor.Run(j + 1, antJ);
        parsed.Add(new No { value = str, indexStart = antJ, indexEnd = j, childs = res, type = opcional ? NoType.colchete : NoType.parenteses });
        buffer.Clear();
        antJ = j;
        j = leitor.j;

        return end;
    }

    int Recursao(char c, string str, int strIndex)
    {
        int end = strIndex + IndexFechamento(c, str[strIndex..]);

        try
        {
            str = str[(strIndex + 1)..end];
        }
        catch (System.Exception e)
        {
            Console.WriteLine($"Try to substring at {(strIndex + 1)} - {end}");
            throw e;
        }

        Leitor leitor = new(str, j, this);

        var res = leitor.Run(j + 1, j);
        parsed.Add(new No { value = str, indexStart = antJ, indexEnd = j, childs = res, type = NoType.chave });
        buffer.Clear();
        antJ = j;
        j = leitor.j;

        return end;
    }


}