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
                { "Bool", "A01" },
                { "Begin", "A02" },
                { "While", "A03" },
                { "Return", "A04" },
                { "Break", "A05" },
                { "False", "A06" },
                { "Void", "A07" },
                { "Program", "A08" },
                { "Char", "A09" },
                { "Float", "A10" },
                { "True", "A11" },
                { "Int", "A12" },
                { "If", "A13" },
                { "Else", "A14" },
                { "String ", "A15" },
                { "End", "A16" },
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
            if (reservedTable.ContainsKey(identifier)) return reservedTable[identifier];
            return null;
        }

    }
}