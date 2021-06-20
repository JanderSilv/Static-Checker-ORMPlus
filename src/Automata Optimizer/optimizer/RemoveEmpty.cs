using System.Collections.Generic;

public class RemoveEmpty : IOptimizer
{
    public List<State> Optimizer(List<State> states)
    {
        foreach (var state in states)
        {

            List<State> empty = state.OnIput("ε");

            while (empty?.Count > 0)
            {
                State s = empty[0];
                if (s.ID != state.ID)
                {
                    foreach (var input in s.Inputs)
                    {
                        state[input] = s[input];
                    }
                }
                if (s.Final) state.Final = true;
                state.RemoveTransition("ε", s);

            }

        }
        return states;
    }
}