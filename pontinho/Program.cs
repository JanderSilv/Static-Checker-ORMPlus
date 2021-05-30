using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


List<string> lines = new();
var txt = File.ReadAllText(args.Length > 1 ? args[1] : "input.txt");
var otxt = txt.Clone();

//pogzinha :D
txt = txt.Replace("(", " ( ").Replace(")", " ) ").Replace("[", " [ ").Replace("]", " ] ").Replace("{", " { ").Replace("}", " } ");

Leitor l = new Leitor(txt);
var res = l.Run();

print(res);

void print(List<No> itens, int pad = 0)
{
    foreach (var item in itens)
    {
        var str = $"{item.indexStart}->{item.value}->{item.indexEnd}";
        str = str.PadLeft(pad);
        lines.Add(str);
        if (item.childs != null) print(item.childs, pad + 1);
    }
}

if (lines.Count > 0)
{
    lines.Insert(0, $"Resultado para: {otxt}\n");
    File.WriteAllLines("out.txt", lines);
}



