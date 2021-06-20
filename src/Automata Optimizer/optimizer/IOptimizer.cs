using System.Collections.Generic;

public interface IOptimizer
{
    List<State> Optimizer(List<State> states);
}