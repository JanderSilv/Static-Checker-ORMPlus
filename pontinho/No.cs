using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum NoType
{
    atomo,
    parenteses,
    colchete,
    chave,
    ou
}
public class No
{
    public string value;
    public int indexEnd;
    public int indexStart;

    public NoType type;
    public List<No> childs = new();

    public int[] childEnd()
    {
        List<int> vals = new List<int>();
        int max = indexStart;
        for (int i = 0; i < childs.Count - 1; i++)
        {
            var item = childs[i];

            if (item.indexEnd > max) max = item.indexEnd;

            if (item.type == NoType.ou)
            {
                vals.Add(max);
                max = indexStart;
            }
        }

        if (childs.Count() > 1)
            vals.Add(max);
        foreach (var item in vals)
        {
            Console.Write($"{item} ");
        }
        Console.WriteLine();
        return vals.ToArray();
    }

    string BuildCases(int[] values, int end)
    {
        StringBuilder sb = new StringBuilder();
        if (values.Length == 0) return "";
        if (values.Length > 1)
        {
            for (int i = 0; i < values.Length - 1; i++)
            {
                sb.Append($"({values[i]},vazio) -> {end}\n");
            }
        }
        sb.Append($"({values[values.Length - 1]},vazio) -> {end}");
        return sb.ToString();
    }
    public override string ToString()
    {
        return type switch
        {
            NoType.atomo => $"({indexStart},{value}) -> {indexEnd}",
            NoType.parenteses => BuildCases(childEnd(), indexEnd),
            NoType.colchete => $"{BuildCases(childEnd(), indexEnd)}\n({indexStart},vazio) -> {indexEnd}",
            NoType.chave => $"{BuildCases(childEnd(), indexEnd)}\n({indexStart},vazio) -> {indexEnd}",
            _ => ""
        };
    }
}