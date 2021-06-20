using System.Collections.Generic;
using System.Linq;

public class RemoveNonAcessible : IOptimizer
{
    public List<State> Optimizer(List<State> states)
    {
        List<State> unknow = new(states.Where(x => x.ID == 0));
        List<State> acessible = new();

        while (unknow.Count > 0)
        {
            State current = unknow[0];

            foreach (State t in current.Childs)
            {
                if (!(unknow.Contains(t) || acessible.Contains(t)))
                {
                    unknow.Add(t);
                }
            }

            unknow.Remove(current);
            acessible.Add(current);
        }

        states = acessible;
        return states;
    }
}