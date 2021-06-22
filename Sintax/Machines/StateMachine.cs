using System.Collections.Generic;
using System.IO;

public class StateMachine
{
    public int state { get; protected set; } = 0;
    protected Dictionary<(int, string), int> rules = new();
    protected List<int> Aceitaveis = new();
    StateMachine current;

    public StateMachine()
    {
        current = this;
    }

    private Dictionary<int, string> WildStates = new();
    private Stack<(int, string)> savedStates = new();

    public bool AceitableState => Aceitaveis.Contains(state);
    public void LoadRules(string path)
    {
        //format state, code, state
        string[] lines = File.ReadAllLines(path);
        foreach (var line in lines)
        {
            string[] p = line.Split(',');
            int from = int.Parse(p[0]);
            int to = int.Parse(p[2]);

            if (p[1] == "FACTOR" || p[1] == "PROGRAM" || p[1] == "STATEMENT")
            {
                if (!WildStates.ContainsKey(from))
                    WildStates.Add(from, p[1]);
            }

            rules.Add((from, p[1]), to);
        }
    }

    public int? NextState(string code)
    {
        int? res = null;
        if (current != this)
            res = current.NextState(code);
        else
            res = current.Next(code);

        if (current != this)
        {
            if (current.AceitableState)
            {
                var (st, cod) = savedStates.Pop();
                if (rules.ContainsKey((st, cod)))
                    res = rules[(st, cod)];
                current = this;
            }
            else if (res == null)
            {
                current = this;
            }
        }

        if (res != null) state = res.Value;
        return res;
    }

    private int? Next(string code)
    {
        if (rules.ContainsKey((state, code)))
        {
            state = rules[(state, code)];
            return state;
        }
        else if (WildStates.ContainsKey(state))
        {
            switch (WildStates[state])
            {
                case "FACTOR":
                    savedStates.Push((state, "FACTOR"));
                    current = new FactortMachine();
                    return current.Next(code);

                case "PROGRAM":
                    savedStates.Push((state, "PROGRAM"));
                    current = new ProgramMachine();
                    return current.Next(code);

                case "STATEMENT":
                    savedStates.Push((state, "STATEMENT"));
                    current = new StatementMachine();
                    return current.Next(code);
            }
        }
        return null;
    }

}