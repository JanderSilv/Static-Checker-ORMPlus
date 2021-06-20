using System;
using System.IO;
using System.Text;

namespace Lexer
{
    public class FileReader
    {
        private string text;
        private int currentIndex = 0;
        public int CurrentLine { get; private set; } = 1;

        public FileReader(string path)
        {
            text = File.ReadAllText(path);
        }
        public char? GetNext()
        {
            if (EOF()) return null;
            char currentChar;

            while (!ValidChar(currentChar = text[currentIndex++]))
            {

            }

            if (currentChar == '\n') CurrentLine++;

            return currentChar;
        }
        public void Rev(int qnt)
        {
            int end = Math.Max(0, currentIndex - qnt);

            for (; currentIndex > end; currentIndex--)
            {
                if (text[currentIndex] == '\n') CurrentLine--;
            }
        }
        private bool EOF()
        {
            return currentIndex >= text.Length;
        }
        public void SkipSingleComment()
        {
            SkipUntil('\n');
        }
        public void SkipMultComment()
        {
            SkipUntil("*/");
        }

        private void SkipUntil(string format)
        {
            int pos = text.IndexOf(format, currentIndex);
            if (pos != -1 || pos != currentIndex)
                for (int i = 0; i < pos; i++) GetNext();
        }
        private void SkipUntil(char format)
        {
            char? c;
            while ((c = GetNext()) != format) { if (c == null) break; };
        }
        private bool ValidChar(char c)
        {
            if ((char.IsLetterOrDigit(c) || "\n!=#&();[]{},<>%/*+-\"$_. ".Contains(c))) return true;

            return false;
        }
    }
}