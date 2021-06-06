using System;
using System.IO;


foreach (var path in Directory.GetFiles("inputs"))
{
    var file = new FileInfo(path);
    var fileName = file.Name.Replace(file.Extension, "");
    Grammar current = new Grammar(path)
    .SaveFile($"outputs/{fileName}/bruto/")
    .SaveTable($"outputs/{fileName}/bruto/")
    .RemoveEmptyTransitions()
    .SaveFile($"outputs/{fileName}/remVazio/")
    .SaveTable($"outputs/{fileName}/remVazio/")
    .RemoveEmptyTransitions()
    .SaveFile($"outputs/{fileName}/remInacessivel/")
    .SaveTable($"outputs/{fileName}/remInacessivel/");


}

