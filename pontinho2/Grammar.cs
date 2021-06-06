using System.Linq;
using System.Collections.ObjectModel;
using System.IO;
using System.Collections.Generic;
using OfficeOpenXml;
using System;

public class Grammar
{

    public ReadOnlyDictionary<string, ReadOnlyCollection<Transicao>> transitions { get => new(_transicoes); }
    public ReadOnlyDictionary<string, ReadOnlyCollection<int>> finals { get => new(_finais); }
    public ReadOnlyCollection<string> NTerminais { get => new(nTerminais); }

    private Dictionary<string, string> inputs = new();
    private Dictionary<string, string> outputs = new();
    private Dictionary<string, ReadOnlyCollection<Transicao>> _transicoes = new();
    private Dictionary<string, ReadOnlyCollection<int>> _finais = new();
    private List<string> nTerminais = new();



    public Grammar(string filePath)
    {


        var file = new FileInfo(filePath);
        var fileName = file.Name.Replace(file.Extension, "");
        string fileData = File.ReadAllText(filePath).Replace('\n', ' ');
        var skip = 0;

        while (skip < fileData.Length)
        {
            string str = string.Concat(fileData.Skip(skip).TakeWhile(x => x != '.')).Trim();
            string nTerminal = string.Concat(str.TakeWhile(x => x != '='));
            string data = string.Concat(str.Skip(nTerminal.Length + 1)).Trim();

            nTerminal = nTerminal.Trim();

            if (string.IsNullOrWhiteSpace(str) || string.IsNullOrWhiteSpace(nTerminal) || string.IsNullOrWhiteSpace(data))
            {
                skip += str.Length + 1;
                continue;
            }

            GramSource source = new(data);
            Reader reader = new(source, 0, 1);


            nTerminais.Add(nTerminal);
            _transicoes[nTerminal] = new(reader.Run());
            _finais[nTerminal] = new(reader.GetFinais());
            inputs[nTerminal] = data;
            outputs[nTerminal] = ".0 " + reader.GetContent();
            skip += str.Length + 1;
        }





    }

    public Grammar SaveFile(string folderPath)
    {
        Directory.CreateDirectory(folderPath);
        foreach (var nt in nTerminais)
        {
            List<string> tofile = new(transitions[nt].Count);
            var atom = transitions[nt].Where(x => x.entrada != "ε").OrderBy(x => x.estado);
            var empty = transitions[nt].Where(x => x.entrada == "ε").OrderBy(x => x.estado);

            tofile.Add($"Entrada:\n{nt} = {inputs[nt]}.");
            tofile.Add($"Saída:\n{nt} = {outputs[nt]}");
            tofile.Add($"\nInício: 0");
            tofile.Add($"Finais: {string.Join(',', finals[nt])}\n");

            tofile.Add("Transicoes:");
            foreach (var item in atom)
            {
                tofile.Add(item.ToString());
            }

            foreach (var item in empty)
            {
                tofile.Add(item.ToString());
            }

            File.WriteAllLines(folderPath + $"{nt}.txt", tofile);
        }

        return this;
    }

    public Grammar SaveTable(string folderPath)
    {
        Directory.CreateDirectory(folderPath);
        foreach (var nt in nTerminais)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var file = new FileInfo(folderPath + $"{nt}.xlsx");
            if (file.Exists)
            {
                file.Delete();
            }

            using var package = new ExcelPackage(file);
            var worksheet = package.Workbook.Worksheets.Add(nt);


            var groups = transitions[nt].GroupBy(x => x.entrada);
            int qntStates = transitions[nt].Max(x => x.proximo) + 1;



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

            if (vazio != null)
            {
                worksheet.Cells[1, collumn].Value = "ε";
                foreach (var t in vazio.OrderBy(x => x.estado).ToArray())
                {
                    worksheet.Cells[t.estado + 2, collumn].Value = t.proximo;

                }
            }

            package.SaveAs(file);
        }

        return this;
    }

    public Grammar RemoveEmptyTransitions()
    {
        foreach (var nt in nTerminais)
        {
            List<Transicao> notEmpty = new(transitions[nt].Where(x => x.entrada != "ε"));
            List<Transicao> empty = new(transitions[nt].Where(x => x.entrada == "ε"));

            while (empty.Count > 0)
            {
                List<Transicao> res = new();

                foreach (var e in empty)
                {
                    List<Transicao> nexts = new();
                    nexts.AddRange(notEmpty.Where(x => x.estado == e.proximo));
                    nexts.AddRange(empty.Where(x => x.estado == e.proximo));

                    if (_finais[nt].Contains(e.proximo) && !_finais[nt].Contains(e.estado)) _finais[nt].Append(e.estado);


                    foreach (var n in nexts)
                    {
                        res.Add(new Transicao() { estado = e.estado, proximo = n.proximo, entrada = n.entrada });
                    }

                }
                empty.Clear();
                empty.AddRange(res.Where(x => x.entrada == "ε"));
                notEmpty.AddRange(res.Where(x => x.entrada != "ε"));
                Console.WriteLine(empty.Count);
            }

            _transicoes[nt] = new(notEmpty);

        }




        return this;
    }

    public Grammar RemoveUnacessible()
    {
        foreach (var nt in nTerminais)
        {
            List<Transicao> tr = new(_transicoes[nt]);

            List<int> acessible = new List<int>() { 0 };
            List<int> considered = new List<int>();

            while (acessible.Count > 0)
            {
                int state = acessible[0];
                acessible.AddRange(tr.Where(x => x.estado == state).Select(x => x.proximo).Distinct());
                considered.Add(state);
                acessible.RemoveAll(x => considered.Contains(x));
            }

            tr.RemoveAll(x => !considered.Contains(x.estado));
            _transicoes[nt] = new(tr);
        }


        return this;
    }
}