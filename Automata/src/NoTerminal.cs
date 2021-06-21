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
        Reader r = new Reader(input);
        Atom current = new Atom().Read(r);
        int jc = 1;
        int lc = 0;
        current.Count(0, ref jc, ref lc);

        List<Transition> transitions = current.GetTransitions().OrderBy(x => x.state).ToList();
        List<int> finals = current.InternalFinals;
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
        this.output = current.ToString();
    }

    public void Use(IOptimizer optimizer)
    {
        states = optimizer.Optimizer(states);
    }
}
