public class ProgramMachine : StateMachine
{
    public ProgramMachine()
    {
        LoadRules("rules/program.rule");
        Aceitaveis = new() { 172 };
    }
}
