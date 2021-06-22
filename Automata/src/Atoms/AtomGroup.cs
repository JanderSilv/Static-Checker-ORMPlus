using System.Collections.Generic;
using System.Linq;

public class AtomGroup : Atom
{
    public AtomGroup()
    {
        type = AtomType.Group;
    }
    public override string ToString() => $"(.{FromState} {string.Join(" | ", Childs.Select(x => string.Join(' ', x)))}).{ToState}";

    public override void Count(int start, ref int current, ref int last)
    {
        indexBefore = last;
        FromState = last;
        ToState = current++;
        start = FromState;

        int st = FromState;
        foreach (var cl in Childs)
        {
            st = FromState;
            last = FromState;
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

        last = ToState;
    }

    public override IEnumerable<Transition> GetTransitions()
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



