using System.Collections.Generic;

public class ReservedSymbol
{
    public int Entry;
    public string Code;
    public string Lexeme;
    public List<int> FirstApearences = new();

    public void AddOcurrency(int line)
    {
        FirstApearences.Add(line);
    }
    public override string ToString()
    {
        return $"{Entry},{Lexeme},{Code},{{{string.Join(',', FirstApearences)}}}";
    }

}