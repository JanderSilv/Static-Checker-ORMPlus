using System;
using System.Collections.Generic;
using System.Linq;

public class RemoveNonDeterministic : IOptimizer
{
    int nextStateID;

    public List<State> Optimizer(List<State> states)
    {
        nextStateID = states.Max(x => x.ID) + 1;

        List<State> nonDet = states.Where(x => !x.Deterministic).ToList();

        while (nonDet.Count > 0)
        {
            foreach (var state in nonDet)
            {
                Dictionary<string, List<State>> nDet = state.GetNonDeterministcTransitions();

                foreach (var nd in nDet)
                {

                    State s = newState(nd.Value);
                    state[nd.Key].Clear();
                    state[nd.Key].Add(s);

                    states.Add(s);
                }


            }
            nonDet = states.Where(x => !x.Deterministic).ToList();

        }

        return states;
    }

    private State newState(List<State> states)
    {
        State nState = new State(nextStateID++);

        foreach (var s in states)
        {
            foreach (var i in s.Inputs)
            {
                nState[i] = s[i];
            }
        }

        return nState;
    }
}