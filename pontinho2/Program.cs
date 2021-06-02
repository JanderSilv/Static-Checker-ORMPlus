using System.IO;


foreach (var path in Directory.GetFiles("inputs"))
{
    new Grammar(path);
}

