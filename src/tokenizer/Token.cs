using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class AToken
{
    public static int Counter { protected set; get; } = 0;
    public int Start { protected set; get; }
    public int End { protected set; get; }
    public virtual string GetContent { get; }
    public List<List<AToken>> Inner = new();

    public override string ToString() => GetContent;

    protected void ReadScoped(TextBuffer buffer, char end)
    {

        if (Inner.Count == 0) Inner.Add(new List<AToken>());
        List<AToken> currentLine = Inner[0];

        char c;
        while ((c = buffer.GetNext()) != end)
        {
            if (c == ' ' || c == '\n') continue;
            if (c == '\0') break;

            if (c == '|')
            {
                currentLine = new();
                Inner.Add(currentLine);
                continue;
            }


            switch (c)
            {
                case '(':
                    {
                        var n = currentLine.Count == 0 ? new TokenGroup(Start, buffer) : new TokenGroup(Counter, buffer);
                        currentLine.Add(n);
                    }
                    break;

                case '[':
                    {
                        var n = currentLine.Count == 0 ? new TokenOption(Start, buffer) : new TokenOption(Counter, buffer);
                        currentLine.Add(n);
                    }
                    break;
                case '{':
                    {
                        var n = new TokenRepeat(buffer);
                        currentLine.Add(n);
                    }
                    break;
                case '\"':
                    {
                        var n = currentLine.Count == 0 ? new TokenTerminal(Start, buffer) : new TokenTerminal(Counter, buffer);
                        currentLine.Add(n);
                    }
                    break;
                default:
                    {
                        var n = currentLine.Count == 0 ? new Token(Start, buffer, c) : new Token(Counter, buffer, c);
                        currentLine.Add(n);
                    }
                    break;
            }

        }
    }

    protected string ParseScoped()
    {
        return string.Join('|', Inner.Select(l => string.Join("", l.Select(t => t.GetContent))));
    }

    public IEnumerable<int> GetFinals => Inner.Select(x => x.Last().End);

    public abstract IEnumerable<Transition> GetTransitions();
}

public class Token : AToken
{
    public override string GetContent => $" {content}.{End} ";
    private string content;
    public Token(int Start, TextBuffer bufffer, char c)
    {
        this.Start = Start;
        content = c + bufffer.GetUntil("\" ([{}])|", consume: false);
        End = ++Counter;
    }

    public override IEnumerable<Transition> GetTransitions()
    {
        return new List<Transition>() { new Transition() { state = Start, input = content, next = End } };
    }
}

public class TokenTerminal : AToken
{
    public override string GetContent => $" \"{content}\".{End} ";
    private string content;
    public TokenTerminal(int Start, TextBuffer bufffer)
    {
        this.Start = Start;
        content = bufffer.GetUntil('\"');
        End = ++Counter;
    }
    public override IEnumerable<Transition> GetTransitions()
    {
        return new List<Transition>() { new Transition() { state = Start, input = $"\"{content}\"", next = End } };
    }
}

public class TokenInit : AToken
{
    public override string GetContent => $".{Start} {ParseScoped()} ";
    public TokenInit(TextBuffer buffer)
    {
        this.Start = 0;
        Counter = 0;

        ReadScoped(buffer, '\0');
    }

    public override IEnumerable<Transition> GetTransitions()
    {
        return Inner.SelectMany(x => x.SelectMany(x => x.GetTransitions()));
    }
}

public class TokenGroup : AToken
{
    public override string GetContent => $" (.{Start} {ParseScoped()} ).{End} ";
    public TokenGroup(int Start, TextBuffer buffer)
    {
        this.Start = Start;
        End = ++Counter;

        ReadScoped(buffer, ')');
    }
    public override IEnumerable<Transition> GetTransitions()
    {
        return Inner.SelectMany(x => x.SelectMany(x => x.GetTransitions()))
        .Concat(GetFinals.Select(i => new Transition() { state = i, input = "ε", next = End }));
    }
}

public class TokenOption : AToken
{
    public override string GetContent => $" [.{Start} {ParseScoped()} ].{End} ";
    public TokenOption(int Start, TextBuffer buffer)
    {
        this.Start = Start;
        End = ++Counter;

        ReadScoped(buffer, ']');
    }
    public override IEnumerable<Transition> GetTransitions()
    {
        return Inner.SelectMany(x => x.SelectMany(x => x.GetTransitions()))
        .Concat(GetFinals.Select(i => new Transition() { state = i, input = "ε", next = End }))
        .Append(new Transition() { state = Start, input = "ε", next = End });
    }
}

public class TokenRepeat : AToken
{
    public override string GetContent => $" {{.{Start} {ParseScoped()} }}.{End} ";
    private int before;
    public TokenRepeat(TextBuffer buffer)
    {
        before = Counter;
        this.Start = ++Counter;
        this.End = Start;

        ReadScoped(buffer, '}');
    }
    public override IEnumerable<Transition> GetTransitions()
    {
        return Inner.SelectMany(x => x.SelectMany(x => x.GetTransitions()))
        .Concat(GetFinals.Select(i => new Transition() { state = i, input = "ε", next = End }))
        .Append(new Transition() { state = before, input = "ε", next = End });
    }
}