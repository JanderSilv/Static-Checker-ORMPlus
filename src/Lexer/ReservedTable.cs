using System.Collections.Generic;

namespace Lexer
{
    public static class ReservedTable
    {
        private static readonly Dictionary<string, string> reservedTable;

        static ReservedTable()
        {
            reservedTable = new()
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
            };
        }

        public static string GetTokenCode(string identifier)
        {
            string str = identifier.ToUpper();
            if (reservedTable.ContainsKey(str)) return reservedTable[str];
            return null;
        }

    }
}