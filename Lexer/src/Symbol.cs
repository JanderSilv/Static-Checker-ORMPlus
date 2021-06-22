using System.Collections.Generic;
using System.Linq;

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
        public void UpdateCode(string code)
        {
            this.Code = code;
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

}