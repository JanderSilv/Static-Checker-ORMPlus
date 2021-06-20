using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class NoTerminal
{
    public readonly string name, input;
    public string Output => output;
    public List<State> States => states;

    private string output;
    private List<State> states;

    public NoTerminal(string name, string input)
    {
        this.name = name;
        this.input = input;
    }

    public void Parse()
    {
        TextBuffer buffer = new(input);
        TokenInit tk = new TokenInit(buffer);
        List<Transition> transitions = tk.GetTransitions().OrderBy(x => x.state).ToList();
        List<int> finals = tk.GetFinals.ToList();
        List<State> _states = transitions.Select(k => k.state).Concat(finals).Distinct().Select(i => new State(i)).ToList();

        foreach (var st in _states)
        {
            st.Final = finals.Contains(st.ID);
            var trs = transitions.Where(x => x.state == st.ID).Select(t => new { t.input, t.next });

            foreach (var tr in trs)
            {
                st[tr.input] = _states.Where(s => s.ID == tr.next).ToList();
            }
        }

        this.states = _states;
        this.output = tk.ToString();
    }

    public void Use(IOptimizer optimizer)
    {
        states = optimizer.Optimizer(states);
    }
}
