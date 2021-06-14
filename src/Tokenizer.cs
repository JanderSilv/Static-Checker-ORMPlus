using System.Text;
using System.Collections.Generic;
using System.Linq;
public enum Context
{
    None,
    Optional,
    Group,
    Recursive
}
public class Tokenizer
{
    static int counter = 0;
    private int initCounter;
    private int lastCounter;
    private int outCounter;

    TextBuffer source;
    Context context;
    List<Transition> finais = new List<Transition>();
    List<int> saidas = new List<int>();
    StringBuilder content = new();

    public Tokenizer(TextBuffer source, int init = 0, int? counter = null, Context context = Context.None)
    {
        this.source = source;
        this.context = context;
        initCounter = init;
        lastCounter = init;

        if (counter != null)
            Tokenizer.counter = counter.Value;

    }
    int NextCounter()
    {
        int c = counter;
        lastCounter = counter++;
        return c;
    }
    void AddFinal(Transition t) { if (t != null) finais.Add(t); }

    Tokenizer SetBound(int i)
    {
        outCounter = i;
        if (context == Context.Recursive)
        {
            lastCounter = outCounter;
            initCounter = lastCounter;
            counter = outCounter + 1;
        }
        return this;
    }
    int GetLast() => (context == Context.None) ? lastCounter : outCounter;

    public List<int> GetFinais() => saidas;

    public string GetContent() => context switch
    {
        Context.None => content.ToString(),
        Context.Group => $"(.{initCounter} {content.ToString()}).{outCounter} ",
        Context.Optional => $"[.{initCounter} {content.ToString()}].{outCounter} ",
        Context.Recursive => $"{{.{initCounter} {content.ToString()} }}.{outCounter} ",
        _ => ""
    };


    public List<Transition> GetTransitions()
    {
        StringBuilder sb = new();
        Transition lastToken = null;
        char currentChar;
        List<Transition> transicoes = new List<Transition>();


        while ((currentChar = source.GetNext()) != '\0')
        {
            if (currentChar == '\0' || currentChar == '\n') continue;
            switch (currentChar)
            {
                case '(':
                    parenteses();
                    continue;
                case '[':
                    colchetes();
                    continue;
                case '{':
                    chaves();
                    continue;

                case ')' or ']' or '}':
                    clearBuffer();
                    handleVoids();
                    saidas.Add(GetLast());
                    return transicoes;
                case '|':
                    ou();
                    continue;
                case ' ':
                    clearBuffer();
                    continue;

                case '"':
                    terminal();
                    continue;

                default:
                    sb.Append(currentChar);
                    continue;
            }
        }
        clearBuffer();

        void parenteses()
        {
            clearBuffer();
            Tokenizer r = new Tokenizer(source, lastCounter, context: Context.Group);
            r.SetBound(NextCounter());
            var res = r.GetTransitions();
            lastCounter = r.GetLast();
            transicoes.AddRange(res);

            content.Append(r.GetContent());
        }
        void colchetes()
        {
            clearBuffer();
            transicoes.Add(new() { input = "ε", state = lastCounter, next = outCounter });
            Tokenizer r = new Tokenizer(source, lastCounter, context: Context.Optional);
            r.SetBound(NextCounter());
            var res = r.GetTransitions();
            lastCounter = r.GetLast();
            transicoes.AddRange(res);

            content.Append(r.GetContent());
        }
        void chaves()
        {
            clearBuffer();
            transicoes.Add(new() { input = "ε", state = lastCounter, next = outCounter });
            Tokenizer r = new Tokenizer(source, 0, context: Context.Recursive);
            r.SetBound(counter);
            var res = r.GetTransitions();
            lastCounter = r.GetLast();
            transicoes.AddRange(res);

            content.Append(r.GetContent());
        }
        void ou()
        {
            clearBuffer();
            AddFinal(lastToken);

            saidas.Add(lastCounter);
            lastCounter = initCounter;

            content.Append($" |.{initCounter} ");
        }
        void terminal()
        {
            string nterminal = source.GetUntil('\"');
            sb.Append(currentChar).Append(nterminal);
            addAtom();
        }

        void handleVoids()
        {


            AddFinal(lastToken);

            if (context != Context.None)
            {
                foreach (var item in finais)
                {
                    transicoes.Add(new() { input = "ε", state = item.next, next = outCounter - 1 });
                }
            }


        }
        void clearBuffer()
        {
            if (sb.Length > 0) addAtom();
        }

        void addAtom()
        {
            string s = sb.ToString();
            if (string.IsNullOrWhiteSpace(s)) return;
            lastToken = new() { state = lastCounter, input = sb.ToString(), next = counter };
            content.Append($"{lastToken.input}.{lastToken.next} ");
            transicoes.Add(lastToken);
            sb.Clear();
            NextCounter();
        }

        handleVoids();

        transicoes.RemoveAll(x => string.IsNullOrEmpty(x.input) || string.IsNullOrWhiteSpace(x.input));
        foreach (var item in transicoes)
        {
            item.input = item.input.Trim();
        }
        saidas.Add(GetLast());
        return transicoes.ToList();
    }
}