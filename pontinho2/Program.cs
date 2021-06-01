using System.IO;


foreach (var path in Directory.GetFiles("inputs"))
{
    var f = new FileInfo(path);
    var n = f.Name.Replace(f.Extension, "");
    Directory.CreateDirectory($"outputs/{n}");

    new Grammar(path).SaveFile($"outputs/{n}/{n}.txt").SaveTable($"outputs/{n}/{n}.xlsx");

}

