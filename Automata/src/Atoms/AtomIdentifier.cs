using System.Collections.Generic;
using System.Linq;

public class AtomIdentifier : Atom
{
    private bool terminal = false;

    public AtomIdentifier(char start)
    {
        type = AtomType.Identifier;
        if (start == '\"') terminal = true;
        else buffer.Append(start);
    }

    public override Atom Read(Reader r)
    {
        char? c = r.GetNext();

        if (terminal)
        {
            while (c != '\"' && c != null)
            {
                buffer.Append(c);
                c = r.GetNext();
            }
            return this;
        }

        while (c != null && (char.IsLetter(c.Value) || c == '-'))
        {
            buffer.Append(c);
            c = r.GetNext();
        }
        r.Rev(1);
        return this;
    }

    public override string ToString() => terminal ? $"{FromState}.\"{input}\".{ToState}" : $"{FromState}.{input}.{ToState}";

    public override void Count(int start, ref int current, ref int last)
    {
        if (start == current) current++;
        FromState = start;
        ToState = current++;
        last = ToState;
    }

    public override IEnumerable<Transition> GetTransitions()
    {
        return new List<Transition>() { new() { input = input, next = ToState, state = FromState } };

    }
}

// public class AtomIdentifier : Atom
// {
//     private bool terminal = false;
//     public AtomIdentifier(Atom a = null) : base(a)
//     {
//         type = AtomType.Identifier;
//         if (buffer.ToString() == "\"") terminal = true;
//     }

//     public override (Atom, Atom) ConsomeChar(char c, Reader r)
//     {
//         if (terminal)
//         {
//             buffer.Append(c);
//             if (c == '\"')
//             {
//                 return (new AtomNone(), this);
//             }

//             char? ic = r.GetNext();
//             while (ic != '\"' && ic != null)
//             {
//                 buffer.Append(ic.Value);
//                 ic = r.GetNext();
//             }
//             return (new AtomNone(), this);
//         }


//         if (char.IsLetter(c) || c == '-')
//         {
//             buffer.Append(c);

//             char? ic = r.GetNext();
//             while (ic != null && (char.IsLetter(ic.Value) || ic == '-'))
//             {
//                 buffer.Append(ic.Value);
//                 ic = r.GetNext();
//             }
//             return (new AtomNone(), this);
//         }

//         return (new AtomNone().ConsomeChar(c, r).Item1, this);


//     }

//     public override string ToString()
//     {
//         return input;
//     }
// }
