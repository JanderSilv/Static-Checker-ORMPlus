public class StatementMachine : StateMachine
{
    public StatementMachine()
    {
        LoadRules("rules/statement.rule");
        Aceitaveis = new() { 258, 549 };
    }
}
