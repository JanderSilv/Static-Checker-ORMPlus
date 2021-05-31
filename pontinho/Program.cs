using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using OfficeOpenXml;

List<string> lines = new();
var txt = File.ReadAllText(args.Length > 1 ? args[1] : "input.txt");

lines.Add($"Resultado para: {txt}\n");
txt = txt.Replace('.', ' ').Replace('\n', ' ').Trim();

Leitor l = new Leitor(txt);
var res = l.Run();

printPontinhos(res);
buscarFinais(res);

var transicoes = Transicoes(res);
List<Transicao> trAtomo = transicoes.Where(x => x.entrada != "ε").ToList();
List<Transicao> trVazio = transicoes.Where(x => x.entrada == "ε").ToList();

printTransicoes(trAtomo);
printTransicoes(trVazio);

if (lines.Count > 0)
{
    File.WriteAllLines("out.txt", lines);
}


saveExcel(transicoes);


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

List<Transicao> Transicoes(List<No> itens)
{
    List<Transicao> GetTransicao(List<No> i)
    {
        List<Transicao> tr = new();
        foreach (var t in i)
        {
            tr.AddRange(t.Transicoes());
            if (t.childs != null) tr.AddRange(GetTransicao(t.childs));
        }
        return tr;
    }

    return GetTransicao(itens);
}

void printTransicoes(IEnumerable<Transicao> itens)
{
    foreach (var t in itens)
    {
        lines.Add($"({t.estado},{t.entrada}) -> {t.prox}");
        //lines.Add($"{t.estado}_{t.entrada}_{t.prox}");
    }
}

void saveExcel(List<Transicao> itens)
{
    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    var file = new FileInfo("./notacao_tabular.xlsx");
    if (file.Exists)
    {
        file.Delete();
    }

    using var package = new ExcelPackage(file);
    var worksheet = package.Workbook.Worksheets.Add("Notacao Tabular");


    var groups = transicoes.GroupBy(x => x.entrada);

    int qntStates = transicoes.Max(x => x.prox) + 1;

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
            worksheet.Cells[t.estado + 2, collumn].Value = t.prox;

        }
        collumn++;
    }

    worksheet.Cells[1, collumn].Value = "ε";

    foreach (var t in vazio.OrderBy(x => x.estado).ToArray())
    {
        worksheet.Cells[t.estado + 2, collumn].Value = t.prox;

    }

    package.SaveAs(file);

}


