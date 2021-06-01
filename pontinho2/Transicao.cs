public class Transicao
{
    public int estado, proximo;
    public string entrada;

    public override string ToString()
    {
        return $"({estado},{entrada})->{proximo}";
    }
}