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
        foreach (var cl in Childs)
        {
            st = start;
            last = st;
            foreach (var c in cl)
            {
                if (c.type == AtomType.Group || c.type == AtomType.Option || c.type == AtomType.Recursive)
                    c.Count(last, ref current, ref last);
                else
                    c.Count(st, ref current, ref last);
                st = current;
            }
        }

    }
}

public class AtomGroup : Atom
{
    public AtomGroup()
    {
        type = AtomType.Group;
    }
    public override string ToString() => $"(.{FromState} {string.Join(" | ", Childs.Select(x => string.Join(' ', x)))}).{ToState}";

    public override void Count(int start, ref int current, ref int last)
    {
        FromState = start;
        ToState = current++;
        last = FromState;

        int st = start;
        foreach (var cl in Childs)
        {
            st = start;
            last = st;
            foreach (var c in cl)
            {
                if (c.type == AtomType.Group || c.type == AtomType.Option || c.type == AtomType.Recursive)
                    c.Count(last, ref current, ref last);
                else
                    c.Count(st, ref current, ref last);
                st = current;
            }
        }

        last = ToState;
    }
}

public class AtomOptional : Atom
{
    public AtomOptional()
    {
        type = AtomType.Option;
    }
    public override string ToString() => $"[.{FromState} {string.Join(" | ", Childs.Select(x => string.Join(' ', x)))}].{ToState}";

    public override void Count(int start, ref int current, ref int last)
    {
        FromState = start;
        ToState = current++;
        last = FromState;
        int st = start;
        foreach (var cl in Childs)
        {
            st = start;
            last = st;
            foreach (var c in cl)
            {
                if (c.type == AtomType.Group || c.type == AtomType.Option || c.type == AtomType.Recursive)
                    c.Count(last, ref current, ref last);
                else
                    c.Count(st, ref current, ref last);
                st = current;
            }
        }

        last = ToState;

    }
}

public class AtomRecursive : Atom
{
    public AtomRecursive()
    {
        type = AtomType.Recursive;
    }
    public override string ToString() => $"{{.{FromState} {string.Join(" | ", Childs.Select(x => string.Join(' ', x)))}}}.{ToState}";

    public override void Count(int start, ref int current, ref int last)
    {
        ToState = current++;
        FromState = ToState;
        last = FromState;
        int st = FromState;
        foreach (var cl in Childs)
        {
            st = FromState;
            last = st;
            foreach (var c in cl)
            {
                if (c.type == AtomType.Group || c.type == AtomType.Option || c.type == AtomType.Recursive)
                    c.Count(last, ref current, ref last);
                else
                    c.Count(st, ref current, ref last);
                st = current;
            }
        }

        last = ToState;
    }
}



