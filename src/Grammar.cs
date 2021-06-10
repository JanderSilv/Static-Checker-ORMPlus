using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;

public class Grammar
{
    readonly string fileName, filePath;
    List<NoTerminal> noTerminals = new();

    public Grammar(string filePath)
    {
        FileInfo fileInfo = new FileInfo(filePath);

        this.filePath = filePath;
        this.fileName = fileInfo.Name.Replace(fileInfo.Extension, "");

        ReadNoTerminals();

        foreach (var nt in noTerminals) nt.Parse();

    }

    public Grammar SaveTxt(string filePath)
    {
        Directory.CreateDirectory(filePath);
        foreach (var nt in noTerminals)
        {
            IEnumerable<Transition> tr = nt.States.SelectMany(x => x.Transitions);
            IEnumerable<Transition> emptyTr = tr.Where(x => x.input == "ε").OrderBy(x => x.state);
            IEnumerable<Transition> nEmptyTr = tr.Where(x => x.input != "ε").OrderBy(x => x.state);

            List<string> tofile = new(tr.Count());

            tofile.Add($"Entrada:\n{nt.name} = {nt.input}");
            tofile.Add($"Saída:\n{nt.name} = {nt.Output}");
            tofile.Add($"\nInício: 0");
            tofile.Add($"Finais: {string.Join(',', nt.States.Where(x => x.Final).Select(x => x.ID))}\n");

            tofile.Add("Transicoes:");
            foreach (var item in nEmptyTr)
            {
                tofile.Add(item.ToString());
            }

            foreach (var item in emptyTr)
            {
                tofile.Add(item.ToString());
            }
            Console.WriteLine(nt.name);
            File.WriteAllLines(filePath + $"{nt.name}.txt", tofile);
        }
        return this;
    }

    public Grammar SaveXlsx(string filePath)
    {
        Directory.CreateDirectory(filePath);

        //  List<string> atoms = noTerminals.SelectMany(x => x.).




        foreach (var nt in noTerminals)
        {
            List<string> atoms = nt.States.SelectMany(x => x.Transitions.Select(x => x.input)).Distinct().ToList();
            atoms.Remove("ε");
            atoms.Add("ε");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var file = new FileInfo(filePath + $"{nt.name}.xlsx");
            if (file.Exists)
            {
                file.Delete();
            }

            using var package = new ExcelPackage(file);
            var worksheet = package.Workbook.Worksheets.Add(nt.name);

            for (int i = 0; i < atoms.Count; i++)
            {
                worksheet.Cells[1, i + 2].Value = atoms[i];
            }

            for (int i = 0; i < nt.States.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = nt.States[i].ID;

                for (int j = 0; j < atoms.Count; j++)
                {
                    var tr = nt.States[i][atoms[j]];
                    if (tr != null)
                    {
                        string v = "";
                        if (tr.Count > 1)
                        {
                            v = $"{{ {string.Join(',', tr.Select(x => x.ID))} }}";
                        }
                        else if (tr.Count == 1)
                        {
                            v = string.Join(',', tr.Select(x => x.ID));
                        }

                        worksheet.Cells[i + 2, j + 2].Value = v;
                    }
                }
            }

            package.SaveAs(file);
        }
        return this;
    }
    private void ReadNoTerminals()
    {
        string fileContent = File.ReadAllText(filePath).Replace('\n', ' ');
        if (string.IsNullOrWhiteSpace(fileContent)) { Console.WriteLine($"File {filePath} is empty"); return; }

        int fileIndex = 0;
        while (fileIndex < fileContent.Length)
        {
            string nt_full = string.Concat(fileContent.Skip(fileIndex).TakeWhile(x => x != '.')).Trim();
            string nt_name = string.Concat(nt_full.Take(nt_full.IndexOf('=')));
            string nt_content = string.Concat(nt_full.Skip(nt_full.IndexOf('=') + 1)).Trim();

            if (string.IsNullOrWhiteSpace(nt_name) || string.IsNullOrWhiteSpace(nt_content)) { fileIndex += nt_full.Length + 1; continue; }

            noTerminals.Add(new(nt_name, nt_content));

            fileIndex += nt_full.Length + 1;
        }
    }
}