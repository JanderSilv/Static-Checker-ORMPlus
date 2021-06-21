using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

public class RemoveEquivalent : IOptimizer
{
    private Dictionary<Tuple<int, int>, bool> results = new();
    private Stack<Tuple<int, int>> stack = new();
    class Comparer : IEqualityComparer<IEnumerable<string>>
    {
        public bool Equals(IEnumerable<string> x, IEnumerable<string> y)
        {
            return x.SequenceEqual(y);
        }

        public int GetHashCode([DisallowNull] IEnumerable<string> obj)
        {
            return obj.GetHashCode();
        }
    }
    private void FindEquivalents(List<State> states)
    {
        results = new();
        stack = new();
        Dictionary<IEnumerable<string>, List<State>> possibleEq = new(new Comparer());

        while (states.Count > 0)
        {
            var inps = states[0].Inputs;

            if (!possibleEq.ContainsKey(inps)) possibleEq.Add(inps, new());

            var eq = states.Where(x => x.Inputs.SequenceEqual(inps));
            possibleEq[inps].AddRange(eq);

            states.RemoveAll(x => eq.Contains(x));

        }


        foreach (var key in possibleEq.Keys)
        {
            int len = possibleEq[key].Count;

            for (int i = 0; i < len; i++)
                for (int j = i + 1; j < len; j++)
                {
                    State a = possibleEq[key][i], b = possibleEq[key][j];
                    Tuple<int, int> tuple = MakeTuple(a, b);
                    stack.Push(tuple);
                    bool eq = key.All(x => Equivalents(a, b, x));
                    stack.Pop();

                    if (!results.ContainsKey(tuple)) results[tuple] = eq;
                }
        }
    }

    private bool Equivalents(State a, State b, string input)
    {
        Tuple<int, int> idTuple = MakeTuple(a, b);

        if (a.ID == b.ID)
        {
            results[idTuple] = true;
            return results[idTuple];
        }

        if (results.ContainsKey(idTuple))
        {
            return results[idTuple];
        }

        if (a[input].SequenceEqual(b[input]))
        {
            results[idTuple] = true;
            return results[idTuple];
        }
        else
        {
            State a1 = a[input].First();
            State b1 = b[input].First();
            var idTuple2 = MakeTuple(a1, b1);

            if (!a1.Inputs.SequenceEqual(b1.Inputs))
            {
                results[idTuple] = false;
                results[idTuple2] = false;
                return results[idTuple];
            }
            else
            {
                var inps = a1.Inputs;
                if (stack.Contains(idTuple2))
                {
                    results[idTuple] = true;
                    return results[idTuple];
                }
                stack.Push(idTuple2);
                results[idTuple] = inps.All(x => Equivalents(a1, b1, x));
                stack.Pop();
                return results[idTuple];
            }


        }


    }

    private Tuple<int, int> MakeTuple(State a, State b)
    {
        int id1 = a.ID;
        int id2 = b.ID;

        if (id1 > id2)
        {
            int aux = id1;
            id1 = id2;
            id2 = aux;
        }
        return new(id1, id2);
    }

    public List<State> Optimizer(List<State> states)
    {
        {
            List<State> finais = states.Where(x => x.Final).ToList();
            List<int> toRemove = new();
            FindEquivalents(new(finais));
            var eqf = results.Where(x => x.Value == true).Select(x => x.Key).Distinct().ToList();

            foreach (var t in eqf)
            {
                if (t.Item1 == t.Item2) continue;
                State keep = states.Single(x => x.ID == t.Item1);
                State discard = states.Single(x => x.ID == t.Item2);

                foreach (var state in states)
                {
                    foreach (var input in state.Inputs)
                    {
                        if (state[input].Contains(discard))
                        {
                            state[input].Remove(discard);
                            state[input].Add(keep);
                        }
                    }
                }
                toRemove.Add(discard.ID);

            }
            states.RemoveAll(x => toRemove.Contains(x.ID));
        }


        {
            List<State> nFinais = states.Where(x => !x.Final).ToList();
            List<int> toRemove = new();
            FindEquivalents(new(nFinais));
            var eqnf = results.Where(x => x.Value == true).Select(x => x.Key).Distinct().ToList();

            foreach (var t in eqnf)
            {
                if (t.Item1 == t.Item2) continue;
                State keep = states.Single(x => x.ID == t.Item1);
                State discard = states.Single(x => x.ID == t.Item2);


                foreach (var state in states)
                {
                    foreach (var input in state.Inputs)
                    {
                        if (state[input].Contains(discard))
                        {
                            state[input].Remove(discard);
                            state[input].Add(keep);
                        }
                    }
                }

                toRemove.Add(discard.ID);
            }
            states.RemoveAll(x => toRemove.Contains(x.ID));
        }

        return states;
    }
}