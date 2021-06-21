// public class AtomNone : Atom
// {
//     public override (Atom, Atom) ConsomeChar(char c, Reader r)
//     {
//         if (char.IsLetter(c) || c == '\"')
//         {
//             buffer.Append(c);
//             return (new AtomIdentifier(this), null);
//         }
//         if (c == '(')
//         {
//             return (new AtomGroup(), null);
//         }
//         if (c == ')')
//         {
//             return (null, null);
//         }

//         return (this, null);
//     }
// }
