using System.IO;
using System.Text;

public class TextBuffer
{
    private string text;
    private int index;

    public TextBuffer(string str)
    {
        text = str;
        index = 0;
    }

    public char GetNext()
    {
        if (index < text.Length)
            return text[index++];
        else return '\0';
    }

    public string GetUntil(char c, bool consume = true)
    {
        StringBuilder sb = new();
        char g = GetNext();
        while (true)
        {
            if (g == c || g == '\0') break;
            sb.Append(g);
            g = GetNext();
        }
        if (!consume) index--;
        return sb.ToString();
    }
    public string GetUntil(string c, bool consume = true)
    {
        StringBuilder sb = new();
        char g = GetNext();
        while (true)
        {
            if (c.Contains(g) || g == '\0') break;
            sb.Append(g);
            g = GetNext();
        }
        if (!consume) index--;
        return sb.ToString();
    }
}