using System;
using static System.Console;
using System.IO;
public class Reader
{

    string text;
    int count = 0;
    public Reader()
    {
        text = File.ReadAllText("gramatica.txt");

    }

    public char? GetNext()
    {
        if (count >= text.Length) return null;
        return text[count++];
    }

    public void Rev(int q)
    {
        count -= q;
        if (count < 0) count = 0;
    }

}
