using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using static System.Console;
public class Grammar
{
    readonly string fileName, filePath;
    List<NoTerminal> noTerminals = new();

    private readonly Dictionary<string, string> reservedTable = new()
    {
        { "BOOL", "A01" },
        { "BEGIN", "A02" },
        { "WHILE", "A03" },
        { "RETURN", "A04" },
        { "BREAK", "A05" },
        { "FALSE", "A06" },
        { "VOID", "A07" },
        { "PROGRAM", "A08" },
        { "CHAR", "A09" },
        { "FLOAT", "A10" },
        { "TRUE", "A11" },
        { "INT", "A12" },
        { "IF", "A13" },
        { "ELSE", "A14" },
        { "STRING", "A15" },
        { "END", "A16" },
        { "!=", "B01" },
        { "!", "B02" },
        { "&", "B03" },
        { "%", "B04" },
        { "(", "B05" },
        { "/", "B06" },
        { ")", "B07" },
        { "*", "B08" },
        { ";", "B09" },
        { "+", "B10" },
        { "[", "B11" },
        { "]", "B12" },
        { "{", "B13" },
        { "|", "B14" },
        { "}", "B16" },
        { ",", "B15" },
        { "<", "B18" },
        { "<=", "B17" },
        { "==", "B20" },
        { "=", "B19" },
        { ">=", "B21" },
        { ">", "B22" },
        { "-", "B23" },
        { "#", "B24" },
        { "IDENTIFIER", "C01" },
        { "CONSTANT-STRING", "C02" },
        { "INTEGER-NUMBER", "C03" },
        { "FUNCTION", "C04" },
        { "CHARACTER", "C05" },
        { "FLOAT-NUMBER", "C06" },
    };
    public Grammar(string filePath)
    {
        FileInfo fileInfo = new FileInfo(filePath);

        this.filePath = filePath;
        this.fileName = fileInfo.Name.Replace(fileInfo.Extension, "");

        ReadNoTerminals();

        foreach (var nt in noTerminals)
        {
            nt.Parse();
        }

    }

    public Grammar SaveTxt(string filePath)
    {
        Directory.CreateDirectory(filePath);
        foreach (var nt in noTerminals)
        {
            IEnumerable<Transition> tr = nt.States.SelectMany(x => x.Transitions);
            IEnumerable<Transition> emptyTr = tr.Where(x => x.input == "??").OrderBy(x => x.state);
            IEnumerable<Transition> nEmptyTr = tr.Where(x => x.input != "??").OrderBy(x => x.state);

            List<string> tofile = new(tr.Count());

            tofile.Add($"Entrada:\n{nt.name} = {nt.input}");
            tofile.Add($"Sa??da:\n{nt.name} ={nt.Output}");
            tofile.Add($"\nIn??cio: 0");
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

            File.WriteAllLines(filePath + $"{nt.name}.txt", tofile);
        }
        return this;
    }

    public Grammar SaveRules(string filePath)
    {
        Directory.CreateDirectory(filePath);
        foreach (var nt in noTerminals)
        {
            IEnumerable<Transition> transitions = nt.States.SelectMany(x => x.Transitions).Where(x => x.input != "??").OrderBy(x => x.state);

            List<string> tofile = new(transitions.Count());

            foreach (var item in transitions)
            {
                tofile.Add($"{item.state},{reservedTable[item.input.ToUpper()]},{item.next}");
            }


            File.WriteAllLines(filePath + $"{nt.name}.rule", tofile);
        }
        return this;
    }

    public Grammar SaveXlsx(string filePath)
    {
        Directory.CreateDirectory(filePath);

        foreach (var nt in noTerminals)
        {
            List<string> atoms = nt.States.SelectMany(x => x.Transitions.Select(x => x.input)).Distinct().ToList();

            atoms.Remove("??");
            atoms.Add("??");

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

                if (nt.States[i].Final)
                {

                    worksheet.Cells[i + 2, 1].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells[i + 2, 1].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells[i + 2, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells[i + 2, 1].Style.Border.Left.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells[i + 2, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[i + 2, 1].Style.Fill.BackgroundColor.SetColor(128, 0, 0, 128);
                }
            }

            package.SaveAs(file);
        }
        return this;
    }

    public Grammar Use(IOptimizer optimizer)
    {
        foreach (var nt in noTerminals)
        {
            nt.Use(optimizer);
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
            if (!reservedTable.ContainsKey(nt_name.ToUpper()))
                reservedTable.Add(nt_name.ToUpper(), nt_name.ToUpper());

            fileIndex += nt_full.Length + 1;
        }
    }
}