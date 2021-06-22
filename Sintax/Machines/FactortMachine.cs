public class FactortMachine : StateMachine
{
    public FactortMachine()
    {
        LoadRules("rules/factor.rule");
        Aceitaveis = new() { 257 };
    }
}