using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

GramSource source = new(File.ReadAllText("inputs/example1.txt").Replace('.', ' ').Replace('\n', ' '));

Reader reader = new Reader(source, 0, 1);

List<Transicao> transicoes = reader.Run();



List<int> finais = reader.GetFinais();

var atom = transicoes.Where(x => x.entrada != "ε");
var vazio = transicoes.Where(x => x.entrada == "ε");

foreach (var item in finais)
{
    Console.Write(item);
    Console.Write(" ");
}
System.Console.WriteLine();


List<string> tofile = new();


tofile.Add(reader.GetContent());

foreach (var item in atom)
{
    tofile.Add(item.ToString());
}

foreach (var item in vazio)
{
    tofile.Add(item.ToString());
}

File.WriteAllLines("outputs/out.txt", tofile);