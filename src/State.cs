using System.Collections.Generic;
using System.Linq;
public class State
{
    public int ID { get; set; }
    public bool Final { get; set; }
    public bool Deterministic => _transitions.All(x => x.Value.Count == 1) && !_transitions.ContainsKey("ε");
    public IEnumerable<Transition> Transitions => _transitions.SelectMany(a => a.Value.Select(s => new Transition() { state = ID, input = a.Key, next = s.ID }));

    public ICollection<State> this[string atom]
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
        _transitions[token].Add(state);
    }

    public void RemoveTransition(string token, State state)
    {
        _transitions[token].Remove(state);
        if (_transitions[token].Count == 0) _transitions.Remove(token);
    }

    public void RemoveTransition(string token) => _transitions.Remove(token);


}