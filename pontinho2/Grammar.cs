using System.Linq;
using System.Collections.ObjectModel;
using System.IO;
using System.Collections.Generic;
using OfficeOpenXml;

public class Grammar
{
    public ReadOnlyCollection<Transicao> transitions;
    public ReadOnlyCollection<int> finals;
    public readonly string parsedString;
    private string fileData;

    public Grammar(string filePath)
    {
        fileData = File.ReadAllText(filePath).Replace('.', ' ').Replace('\n', ' ');

        GramSource source = new(fileData);
        Reader r = new(source, 0, 1);

        var t = r.Run();
        var f = r.GetFinais();
        parsedString = r.GetContent();

        transitions = new ReadOnlyCollection<Transicao>(t);
        finals = new ReadOnlyCollection<int>(f);

    }

    public Grammar SaveFile(string folderPath)
    {
        List<string> tofile = new(transitions.Count);
        var atom = transitions.Where(x => x.entrada != "ε");
        var empty = transitions.Where(x => x.entrada == "ε");

        tofile.Add($"Entrada:\n{fileData}");
        tofile.Add($"Saída:\n{parsedString}");
        tofile.Add($"\nInício: 0");
        tofile.Add($"Finais: {string.Join(',', finals)}\n");

        tofile.Add("Transicoes:");
        foreach (var item in atom)
        {
            tofile.Add(item.ToString());
        }

        foreach (var item in empty)
        {
            tofile.Add(item.ToString());
        }

        File.WriteAllLines(folderPath, tofile);
        return this;
    }

    public Grammar SaveTable(string folderPath)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        var file = new FileInfo(folderPath);
        if (file.Exists)
        {
            file.Delete();
        }

        using var package = new ExcelPackage(file);
        var worksheet = package.Workbook.Worksheets.Add("Notacao Tabular");


        var groups = transitions.GroupBy(x => x.entrada);
        int qntStates = transitions.Max(x => x.proximo) + 1;



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
        return this;
    }
}