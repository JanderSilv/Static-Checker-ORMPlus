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
        Tokenizer tokenizer = new Tokenizer(buffer, counter: 1);

        List<Transition> transitions = tokenizer.GetTransitions();
        List<int> finals = tokenizer.GetFinais();

        var trGroup = transitions.GroupBy(x => x.state);

        List<State> _states = trGroup.Select(k => new State(k.Key)).ToList();

        foreach (var st in _states)
        {
            st.Final = finals.Contains(st.ID);
            var atomGroup = trGroup.Where(x => x.Key == st.ID).Single().GroupBy(x => x.input);
            foreach (var atom in atomGroup)
            {
                st[atom.Key] = atom.Select(x => x.next).SelectMany(i => _states.Where(s => s.ID == i).ToList()).ToList();
            }
        }

        this.states = _states;
        this.output = tokenizer.GetContent();
    }
}
