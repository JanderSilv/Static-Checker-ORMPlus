using System.Text;
using System.Collections.Generic;
using System.Linq;

public enum AtomType
{
    None,
    Identifier,
    Group,
    Option,
    Recursive
}

public class Atom
{
    public int FromState, ToState;
    public AtomType type;

    protected int indexBefore = 0;

    protected StringBuilder buffer;
    public List<List<Atom>> Childs = new();
    public List<int> InternalFinals { get => Childs.Select(x => x.Last().ToState).ToList(); }
    public string input { get => buffer.ToString(); }

    private List<Atom> CurrentChildList;

    public Atom()
    {
        type = AtomType.None;
        buffer = new();
        CurrentChildList = new();
        Childs.Add(CurrentChildList);
    }

    public virtual Atom Read(Reader r)
    {
        char? c;
        while ((c = r.GetNext()) != null)
        {
            if (c == '\"' || char.IsLetter(c.Value) || c == '-')
            {
                Atom f = new AtomIdentifier(c.Value).Read(r);
                CurrentChildList.Add(f);
            }
            if (c == '(')
            {
                Atom f = new AtomGroup().Read(r);
                CurrentChildList.Add(f);
            }

            if (c == '[')
            {
                Atom f = new AtomOptional().Read(r);
                CurrentChildList.Add(f);
            }

            if (c == '{')
            {
                Atom f = new AtomRecursive().Read(r);
                CurrentChildList.Add(f);
            }

            if (c == ']' || c == ')' || c == '}')
            {
                return this;
            }

            if (c == '|')
            {
                CurrentChildList = new();
                Childs.Add(CurrentChildList);
            }

        }

        return this;
    }

    public override string ToString() => $"{string.Join(" | ", Childs.Select(x => string.Join(' ', x)))}";

    public virtual void Count(int start, ref int current, ref int last)
    {
        int st = start;
        indexBefore = last;
        foreach (var cl in Childs)
        {
            st = start;
            last = st;
            foreach (var c in cl)
            {
                if (c.type == AtomType.Group || c.type == AtomType.Option || c.type == AtomType.Recursive)
                {
                    c.Count(last, ref current, ref last);
                    st = current;
                }
                else
                {
                    c.Count(last, ref current, ref last);
                    st = last;
                }
            }
        }

    }

    public virtual IEnumerable<Transition> GetTransitions()
    {

        List<Transition> tr = new();

        foreach (var cl in Childs)
        {
            foreach (var c in cl)
            {
                var t = c.GetTransitions();
                tr.AddRange(t);
            }
        }

        var finals = InternalFinals;
        var f = finals.Select(i => new Transition() { state = i, input = "ε", next = ToState });
        tr.AddRange(f);

        return tr;
    }
}



