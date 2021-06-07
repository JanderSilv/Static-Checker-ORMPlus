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
                var tr = g.OrderBy(x => x.estado).GroupBy(x => x.estado).ToArray();


                foreach (var t in tr)
                {
                    worksheet.Cells[t.Key + 2, collumn].Value = string.Join(',', t.Select(x => x.proximo));

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

    public Grammar RemoveNonDeterministics()
    {

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

    public Grammar EquivalentStates()
    {

        Tuple<List<int>, Dictionary<string[], Tuple<int, int>[]>> FindEquivalents(List<Transicao> final_trans)
        {
            List<int> final_states = final_trans.Select(x => x.estado).Distinct().ToList(); // estados finais
            Dictionary<string[], Tuple<int, int>[]> finals_match = new Dictionary<string[], Tuple<int, int>[]>(new MyEqualityComparer());

            while (final_states.Count > 0)
            {
                int state = final_states[0];

                string[] cons = final_trans.Where(x => x.estado == state).Select(x => x.entrada).Distinct().OrderBy(k => k).ToArray<string>(); //estado x consome y tokens


                int[] similar = final_states.Where(i => final_trans.Where(x => x.estado == i).Select(x => x.entrada).Distinct().OrderBy(k => k).SequenceEqual(cons))
                .Append(state).Distinct().ToArray<int>();

                List<Tuple<int, int>> tmp = new();

                for (int i = 0; i < similar.Length - 1; i++)
                {
                    for (int j = i + 1; j < similar.Length; j++)
                    {
                        int a = similar[i];
                        int b = similar[j];
                        if (b < a)
                        {
                            int aux = b;
                            b = a;
                            a = aux;
                        }
                        tmp.Add(new(a, b));
                    }
                }

                finals_match[cons] = tmp.ToArray();

                final_states.RemoveAll(x => similar.Contains(x));
            }


            return new(final_states, finals_match);
        }

        foreach (var nt in nTerminais)
        {
            List<Transicao> final_trans = _transicoes[nt].Where(x => _finais[nt].Contains(x.estado)).ToList();
            List<Transicao> nfinal_trans = _transicoes[nt].Where(x => !_finais[nt].Contains(x.estado)).ToList();

            var (final_states, final_eqs) = FindEquivalents(final_trans);
            var (nfinal_states, nfinal_eqs) = FindEquivalents(nfinal_trans);


            List<Tuple<int, int>> equivalents = new();
            List<Tuple<int, int>> nequivalents = new();
            Stack<Tuple<int, int>> stack = new();

            int ProxState(int state, string atom) =>
               _transicoes[nt].Where(x => x.estado == state && x.entrada == atom).Select(x => x.proximo).First();

            Tuple<bool, string[]> SameGroup(Tuple<int, int> tuple)
            {
                foreach (var item in final_eqs)
                {
                    Tuple<int, int>[] states = item.Value;
                    if (states.Contains(tuple) || states.Contains(new(tuple.Item2, tuple.Item1))) return new(true, item.Key);
                }
                foreach (var item in nfinal_eqs)
                {
                    Tuple<int, int>[] states = item.Value;
                    if (states.Contains(tuple) || states.Contains(new(tuple.Item2, tuple.Item1))) return new(true, item.Key);
                }
                return new(false, null);
            }

            bool checkEq(Tuple<int, int> tuple, string atom)
            {
                if (stack.Contains(tuple))
                {
                    equivalents.Add(tuple);
                    return true;
                }

                if (equivalents.Contains(tuple)) { return true; }
                if (nequivalents.Contains(tuple)) { return false; }

                int prox1 = ProxState(tuple.Item1, atom);
                int prox2 = ProxState(tuple.Item2, atom);

                if (prox2 < prox1)
                {
                    int aux = prox2;
                    prox2 = prox1;
                    prox1 = aux;
                }

                Tuple<int, int> ntuple = new(prox1, prox2);
                if (equivalents.Contains(ntuple)) { return true; }
                if (nequivalents.Contains(ntuple)) { return false; }

                if (prox1 == prox2)
                {
                    equivalents.Add(ntuple);
                    return true;
                }

                var (sameGroup, atoms) = SameGroup(ntuple);
                if (sameGroup)
                {
                    stack.Push(tuple);
                    bool res = checkState(ntuple, atoms);
                    stack.Pop();

                    return res;
                }
                else
                {
                    nequivalents.Add(ntuple);
                    return false;
                }

            }

            bool checkState(Tuple<int, int> state, string[] atoms)
            {
                foreach (var a in atoms)
                {
                    if (!checkEq(state, a))
                    {
                        nequivalents.Add(state);
                        return false;
                    }
                }
                equivalents.Add(state);
                return true;
            }

            foreach (var item in final_eqs)
            {
                string[] atoms = item.Key;
                Tuple<int, int>[] states = item.Value;

                foreach (var state in states)
                {
                    checkState(state, atoms);
                }
            }

            foreach (var item in nfinal_eqs)
            {
                string[] atoms = item.Key;
                Tuple<int, int>[] states = item.Value;

                foreach (var state in states)
                {
                    checkState(state, atoms);
                }
            }

            Console.WriteLine("\nEquivalentes");
            foreach (var item in equivalents.Distinct())
            {
                Console.WriteLine($"({item.Item1},{item.Item2})");
            }
        }

        return this;
    }
}
