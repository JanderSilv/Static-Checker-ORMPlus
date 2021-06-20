using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class State
{
    public int ID { get; set; }
    public bool Final { get; set; }
    public bool Deterministic => _transitions.All(x => x.Value.Count <= 1) && (this["ε"] == null || this["ε"].Count == 0);
    public IEnumerable<Transition> Transitions => _transitions.SelectMany(a => a.Value.Select(s => new Transition() { state = ID, input = a.Key, next = s.ID }));

    public IEnumerable<State> Childs => _transitions.SelectMany(a => a.Value).Distinct();
    public IEnumerable<string> Inputs => _transitions.Keys;
    public List<State> this[string atom]
    {
        get => _transitions.ContainsKey(atom) ? _transitions[atom] : null;
        set
        {
            if (!_transitions.ContainsKey(atom)) _transitions[atom] = new List<State>();
            _transitions[atom].AddRange(value);
        }
    }


    private Dictionary<string, List<State>> _transitions = new();

    public State(int ID)
    {
        this.ID = ID;
    }

    public void AddTransition(string token, State state)
    {
        if (!_transitions.ContainsKey(token)) _transitions[token] = new();
        _transitions[token].Add(state);
    }

    public void RemoveTransition(string token, State state)
    {
        if (_transitions.ContainsKey(token) && _transitions[token].Count > 0)
        {
            _transitions[token].Remove(state);
        }
        if (_transitions[token].Count == 0) _transitions.Remove(token);
    }

    public void RemoveTransition(string token) => _transitions.Remove(token);
    public void RemoveTransition(State state)
    {
        foreach (var kp in _transitions)
        {
            if (_transitions[kp.Key].Contains(state)) _transitions[kp.Key].Remove(state);
        }
    }

    public List<State> OnIput(string input)
    {
        if (_transitions.ContainsKey(input) && _transitions[input].Count > 0) return _transitions[input];
        return null;
    }

    public Dictionary<string, List<State>> GetNonDeterministcTransitions()
    {
        return new(_transitions.Where(x => x.Value.Count > 1));
    }
}