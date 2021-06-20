using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lexer
{
    public class Symbol
    {
        public int TableEntry;
        public string Lexeme { get; private set; }
        public string Code { get; private set; }
        public string Type { get; private set; }
        public List<int> FirstApearences { get; private set; }
        public int OriginalLen;
        public int TruncatedLen;


        public Symbol(Atom a)
        {
            Lexeme = a.Lexeme;
            Code = a.Code;
            FirstApearences = new();
            FirstApearences.Add(a.LineOcurrency);
            OriginalLen = a.OriginalLenght;
            TruncatedLen = a.TruncatedLenght;
            Type = ReservedTable.GetTokenType(Code);
        }
        public void AddOcurrency(int line)
        {

            if (FirstApearences.Count < 5) FirstApearences.Add(line);
        }
        public void AddOcurrency(IEnumerable<int> lines)
        {

            if (FirstApearences.Count < 5)
            {
                FirstApearences.AddRange(lines);
                FirstApearences = new(FirstApearences.Take(5));
            }
        }
        public override string ToString()
        {
            return $"{TableEntry},{Lexeme},{Code},{Type},{OriginalLen},{TruncatedLen},{{{string.Join(',', FirstApearences)}}}";
        }
    }
    public static class SymbolTable
    {
        public static Dictionary<string, Symbol> symbolTable = new();
        private static int entryCount = 0;
        public static void AddSymbol(Symbol symbol)
        {
            if (symbolTable.ContainsKey(symbol.Lexeme))
            {
                Symbol _symbol = symbolTable[symbol.Lexeme];
                _symbol.AddOcurrency(symbol.FirstApearences);
                _symbol.OriginalLen = symbol.OriginalLen;
                _symbol.TruncatedLen = symbol.TruncatedLen;

            }
            else
            {
                symbol.TableEntry = entryCount++;
                symbolTable[symbol.Lexeme] = symbol;
            }
        }
        public static int? ExistSymbol(string identifier)
        {
            var s = symbolTable.SingleOrDefault(x => x.Key == identifier).Value;
            if (s != null) return s.TableEntry;
            return null;
        }

        public static new string ToString()
        {
            StringBuilder sb = new();
            foreach (var item in symbolTable.Values)
            {
                sb.Append(item.ToString()).Append('\n');
            }
            return sb.ToString();
        }
    }

}