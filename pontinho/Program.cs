using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


List<string> lines = new();
var txt = File.ReadAllText(args.Length > 1 ? args[1] : "input.txt");

lines.Add($"Resultado para: {txt}\n");

txt = txt.Replace('.', ' ').Trim();

Leitor l = new Leitor(txt);
var res = l.Run();


printPontinhos(res);
buscarFinais(res);
printTransicoes(res);

void buscarFinais(List<No> itens)
{
    List<int> finais = new();
    for (int i = 0; i < itens.Count; i++)
    {
        if (itens[i].type == NoType.ou)
        {
            finais.Add(itens[i - 1].indexEnd);
        }
    }
    if (itens[itens.Count - 1].type != NoType.ou)
        finais.Add(itens[itens.Count - 1].indexEnd);

    StringBuilder sb = new("Finais:");
    foreach (var item in finais)
    {
        sb.Append($" {item}");
    }
    sb.Append("\n");
    lines.Add("Inicio: 0\n");
    lines.Add(sb.ToString());
}

void printPontinhos(List<No> itens)
{
    StringBuilder sb = new StringBuilder();
    sb.Append(".0 ");
    for (int i = 0; i < itens.Count; i++)
    {
        var item = itens[i];
        if (i == itens.Count - 1 && item.type == NoType.ou) break;
        sb.Append($"{item.ToString() }");
    }
    sb.Append('\n');
    lines.Add(sb.ToString());
}

void printTransicoes(List<No> itens, int pad = 0)
{
    foreach (var item in itens)
    {
        var str = item.Transicoes();
        str = str.PadLeft(pad);
        lines.Add(str);
        if (item.childs != null) printTransicoes(item.childs, pad + 1);
    }
}

if (lines.Count > 0)
{

    File.WriteAllLines("out.txt", lines);
}



