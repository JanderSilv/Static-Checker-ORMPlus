using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfficeOpenXml;

string file = File.ReadAllText("inputs/example1.txt").Replace('.', ' ').Replace('\n', ' ');
GramSource source = new(file);

Reader reader = new Reader(source, 0, 1);

List<Transicao> transicoes = reader.Run();



List<int> finais = reader.GetFinais();

var atom = transicoes.Where(x => x.entrada != "ε");
var vazio = transicoes.Where(x => x.entrada == "ε");


StringBuilder sb = new();
foreach (var item in finais)
{
    sb.Append(item);
    sb.Append(" ");
}



List<string> tofile = new();

tofile.Add("Entrada:");
tofile.Add(file);
tofile.Add("");

tofile.Add("Entrada Processada:");
tofile.Add(reader.GetContent());
tofile.Add("");

// tofile.Add("Início: 0");
// tofile.Add($"Finais: {sb.ToString()}");
// tofile.Add("");

tofile.Add("Transicoes:");
foreach (var item in atom)
{
    tofile.Add(item.ToString());
}

foreach (var item in vazio)
{
    tofile.Add(item.ToString());
}

File.WriteAllLines("outputs/out.txt", tofile);
saveExcel(transicoes);

void saveExcel(List<Transicao> itens)
{
    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    var file = new FileInfo("outputs/out.xlsx");
    if (file.Exists)
    {
        file.Delete();
    }

    using var package = new ExcelPackage(file);
    var worksheet = package.Workbook.Worksheets.Add("Notacao Tabular");


    var groups = transicoes.GroupBy(x => x.entrada);

    int qntStates = transicoes.Max(x => x.proximo) + 1;

    Console.WriteLine($"Quantidade Estados: {qntStates}");

    for (int i = 0; i < qntStates; i++)
    {
        worksheet.Cells[i + 2, 1].Value = i;
    }

    int collumn = 2;
    IGrouping<string, Transicao> vazio = null;
    foreach (var g in groups)
    {
        string key = g.Key;
        if (key == "ε")
        {
            vazio = g;
            continue;
        }

        worksheet.Cells[1, collumn].Value = key;
        var tr = g.OrderBy(x => x.estado).ToArray();


        foreach (var t in tr)
        {
            worksheet.Cells[t.estado + 2, collumn].Value = t.proximo;

        }
        collumn++;
    }

    worksheet.Cells[1, collumn].Value = "ε";

    foreach (var t in vazio.OrderBy(x => x.estado).ToArray())
    {
        worksheet.Cells[t.estado + 2, collumn].Value = t.proximo;

    }

    package.SaveAs(file);

}