using System.Text;
using System.Collections.Generic;
using System.Linq;

public class Reader
{
    static int counter = 0;
    private int initCounter;
    private int lastCounter;
    private int outCounter;

    GramSource source;
    Context context;
    List<Transicao> finais = new List<Transicao>();
    List<int> saidas = new List<int>();
    StringBuilder content = new();

    public Reader(GramSource source, int init = 0, int? counter = null, Context context = Context.None)
    {
        this.source = source;
        this.context = context;
        initCounter = init;
        lastCounter = init;

        if (counter != null)
            Reader.counter = counter.Value;

    }

    public Reader SetBound(int i)
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

    int NextCounter()
    {
        int c = counter;
        lastCounter = counter++;
        return c;
    }

    public List<int> GetFinais()
    {
        return saidas;
    }

    public int GetLast()
    {
        if (context == Context.None) return lastCounter;
        return outCounter;
    }
    public string GetContent()
    {
        return context switch
        {
            Context.None => content.ToString(),
            Context.Group => $"(.{initCounter} {content.ToString()}).{outCounter} ",
            Context.Optional => $"[.{initCounter} {content.ToString()}].{outCounter} ",
            Context.Recursive => $"{{.{initCounter} {content.ToString()} }}.{outCounter} ",
            _ => ""
        };
    }
    public void AddFinal(Transicao t) { if (t != null) finais.Add(t); }
    public List<Transicao> Run()
    {
        StringBuilder sb = new();
        Transicao lastToken = null;
        char currentChar;
        List<Transicao> transicoes = new List<Transicao>();


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
            Reader r = new Reader(source, lastCounter, context: Context.Group);
            r.SetBound(NextCounter());
            var res = r.Run();
            lastCounter = r.GetLast();
            transicoes.AddRange(res);

            content.Append(r.GetContent());
        }
        void colchetes()
        {
            clearBuffer();
            transicoes.Add(new() { entrada = "ε", estado = lastCounter, proximo = counter });
            Reader r = new Reader(source, lastCounter, context: Context.Optional);
            r.SetBound(NextCounter());
            var res = r.Run();
            lastCounter = r.GetLast();
            transicoes.AddRange(res);

            content.Append(r.GetContent());
        }
        void chaves()
        {
            clearBuffer();
            transicoes.Add(new() { entrada = "ε", estado = lastCounter, proximo = counter });
            Reader r = new Reader(source, 0, context: Context.Recursive);
            r.SetBound(counter);
            var res = r.Run();
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
                    transicoes.Add(new() { entrada = "ε", estado = item.proximo, proximo = outCounter - 1 });
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
            lastToken = new() { estado = lastCounter, entrada = sb.ToString(), proximo = counter };
            content.Append($"{lastToken.entrada}.{lastToken.proximo} ");
            transicoes.Add(lastToken);
            sb.Clear();
            NextCounter();
        }

        handleVoids();

        transicoes.RemoveAll(x => string.IsNullOrEmpty(x.entrada) || string.IsNullOrWhiteSpace(x.entrada));
        foreach (var item in transicoes)
        {
            item.entrada = item.entrada.Trim();
        }
        saidas.Add(GetLast());
        return transicoes;
    }
}