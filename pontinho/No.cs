using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public struct Transicao
{
    public int estado, prox;
    public string entrada;
}

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

    string ResolveChilds()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < childs.Count; i++)
        {
            var item = childs[i];
            if (i == childs.Count - 1 && item.type == NoType.ou) break;
            sb.Append($"{item.ToString() }");
        }
        return sb.ToString();
    }

    public override string ToString()
    {
        return type switch
        {
            NoType.atomo => $"{value}.{indexEnd} ",
            NoType.parenteses => $"(.{indexStart} {ResolveChilds()}).{indexEnd} ",
            NoType.colchete => $"[.{indexStart} {ResolveChilds()}].{indexEnd} ",
            NoType.chave => $"{{.{indexEnd} {ResolveChilds()} }}.{indexEnd} ",
            NoType.ou => $"|.{indexEnd} ",
            _ => ""
        };
    }
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

        return vals.ToArray();
    }


    public List<Transicao> TransicaoVazio(int[] values, int end)
    {
        List<Transicao> list = new List<Transicao>();

        if (values.Length > 1)
        {
            for (int i = 0; i < values.Length - 1; i++)
            {
                list.Add(new Transicao() { estado = values[i], entrada = "ε", prox = end });
            }
        }
        list.Add(new Transicao() { estado = values[values.Length - 1], entrada = "ε", prox = end });

        return list;
    }


    public IEnumerable<Transicao> Transicoes()
    {
        List<Transicao> list = new List<Transicao>();

        switch (type)
        {
            case NoType.atomo:
                list.Add(new Transicao() { entrada = value, estado = indexStart, prox = indexEnd });
                break;
            case NoType.parenteses:
                list.AddRange(TransicaoVazio(childEnd(), indexEnd));
                break;
            case NoType.colchete:
            case NoType.chave:
                list.AddRange(TransicaoVazio(childEnd(), indexEnd));
                list.Add(new Transicao() { entrada = "ε", estado = indexStart, prox = indexEnd });
                break;

        }



        return list;
    }
}