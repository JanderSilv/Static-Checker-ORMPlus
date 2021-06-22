using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lexer
{
    public static class SymbolTable
    {
        public static Dictionary<string, Symbol> symbolTable = new();
        public static Dictionary<string, ReservedSymbol> reservedTable = new();



        public static void Init()
        {
            symbolTable = new();
            reservedTable = new();
        }

        public static void AddSymbol(Atom atom)
        {
            if (atom.Identifier)
            {
                Symbol symbol = new Symbol(atom);
                if (symbolTable.ContainsKey(symbol.Lexeme))
                {
                    Symbol _symbol = symbolTable[symbol.Lexeme];
                    _symbol.AddOcurrency(symbol.FirstApearences);
                    _symbol.OriginalLen = symbol.OriginalLen;
                    _symbol.TruncatedLen = symbol.TruncatedLen;

                }
                else
                {
                    symbol.TableEntry = symbolTable.Count;
                    symbolTable[symbol.Lexeme] = symbol;
                }
            }
            else
            {
                if (reservedTable.ContainsKey(atom.Lexeme))
                {
                    ReservedSymbol rs = reservedTable[atom.Lexeme];
                    rs.AddOcurrency(atom.LineOcurrency);
                }
                else
                {
                    ReservedSymbol rs = new ReservedSymbol();
                    rs.Code = atom.Code;
                    rs.Lexeme = atom.Lexeme;
                    rs.Entry = reservedTable.Count;
                    rs.AddOcurrency(atom.LineOcurrency);

                    reservedTable.Add(rs.Lexeme, rs);
                }
            }
        }
        public static Symbol GetSymbol(int index)
        {
            return symbolTable.Single(x => x.Value.TableEntry == index).Value;
        }
        public static void UpdateSymbol(Symbol s)
        {
            symbolTable[s.Lexeme] = s;
        }
        public static int? ExistSymbol(string identifier)
        {
            var s = symbolTable.SingleOrDefault(x => x.Key == identifier).Value;
            if (s != null) return s.TableEntry;
            return null;
        }

        private static string[] GetRelHeader()
        {
            const string header = @"EQUIPE    E05

COMPONENTES:     
Nome                    Email                                     Telefone
Jander Almeida Silva    jander.silva@aln.senaicimatec.com.br     (71) 99961-6841
Jafe Ferreira           jafe.ferreira@aln.senaicimatec.edu.br    (71) 8815-3915
Ruan Nilton Azevedo     ruan.azevedo@aln.senaicimatec.edu.br     (75) 8334-6684";

            return header.Split('\n').Append("\n").Append("\n").ToArray();
        }
        public static string GetTab()
        {
            string padVal(object s, int len)
            {
                return s.ToString().PadRight(len);
            }

            int entryLen, codeLen, lexemeLen, typeLen, faLen, origLen, truncLen;

            var values = symbolTable.Values.ToList();

            entryLen = Math.Max(values.Select(v => v.TableEntry.ToString().Length).Max(), "Entrada".Length);
            lexemeLen = Math.Max(values.Select(v => v.Lexeme.ToString().Length).Max(), "Lexema".Length);
            codeLen = Math.Max(values.Select(v => v.Code.ToString().Length).Max(), "Codigo".Length);
            typeLen = Math.Max(values.Select(v => v.Type.Length).Max(), "Tipo".Length);
            origLen = Math.Max(values.Select(v => v.OriginalLen.ToString().Length).Max(), "Tam. Original".Length);
            truncLen = Math.Max(values.Select(v => v.TruncatedLen.ToString().Length).Max(), "Tam. Truncado".Length);
            faLen = Math.Max(values.Select(v => string.Join(',', v.FirstApearences.Take(5)).Length).Max(), "Aparicoes(5)".Length);

            List<string> lines = new();
            lines.AddRange(GetRelHeader());
            string tmp = $"{padVal("Entrada", entryLen)}|{padVal("Lexema", lexemeLen)}|{padVal("Codigo", codeLen)}|{padVal("Tipo", typeLen)}|{padVal("Tam. Original", origLen)}|{padVal("Tam. Truncado", truncLen)}|{padVal("Aparicoes(5)", faLen)}";
            lines.Add(tmp);
            string line = "";
            for (int i = 0; i < tmp.Length; i++)
            {
                line += '-';
            }
            lines.Add(line);

            foreach (var v in values)
            {
                lines.Add($"{padVal(v.TableEntry, entryLen)}|{padVal(v.Lexeme, lexemeLen)}|{padVal(v.Code, codeLen)}|{padVal(v.Type, typeLen)}|{padVal(v.OriginalLen, origLen)}|{padVal(v.TruncatedLen, truncLen)}|{padVal(string.Join(',', v.FirstApearences.Take(5)), faLen)}");
            }

            return string.Join('\n', lines);
        }

        public static string GetLex()
        {
            string padVal(object s, int len)
            {
                return s.ToString().PadRight(len);
            }

            int entryLen, codeLen, lexemeLen, countLen, faLen;

            var values = reservedTable.Values.ToList();

            entryLen = Math.Max(values.Select(v => v.Entry.ToString().Length).Max(), "Entrada".Length);
            codeLen = Math.Max(values.Select(v => v.Code.ToString().Length).Max(), "Codigo".Length);
            lexemeLen = Math.Max(values.Select(v => v.Lexeme.ToString().Length).Max(), "Lexema".Length);
            countLen = Math.Max(values.Select(v => v.FirstApearences.Count().ToString().Length).Max(), "Qnt".Length);
            faLen = Math.Max(values.Select(v => string.Join(',', v.FirstApearences.Take(5)).Length).Max(), "Aparicoes(5)".Length);

            List<string> lines = new();
            lines.AddRange(GetRelHeader());
            string tmp = $"{padVal("Entrada", entryLen)}|{padVal("Lexema", lexemeLen)}|{padVal("Codigo", codeLen)}|{padVal("Qnt", countLen)}|{padVal("Aparicoes(5)", faLen)}";
            lines.Add(tmp);
            string line = "";
            for (int i = 0; i < tmp.Length; i++)
            {
                line += '-';
            }
            lines.Add(line);

            foreach (var item in values)
            {
                lines.Add($"{padVal(item.Entry, entryLen)}|{padVal(item.Lexeme, lexemeLen)}|{padVal(item.Code, codeLen)}|{padVal(item.FirstApearences.Count, countLen)}|{padVal(string.Join(',', item.FirstApearences.Take(5)), faLen)}");
            }

            return string.Join('\n', lines);
        }

    }

}