using System;
using System.IO;
using System.Collections.Generic;

string input_path = "./input";
List<string> files_to_read = new();

if (args.Length > 1)
{
    for (int i = 1; i < args.Length; i++)
    {
        input_path = args[i];

        if (Directory.Exists(input_path))
        {
            files_to_read.AddRange(Directory.GetFiles(input_path));
        }
        else if (File.Exists(input_path)) files_to_read.Add(input_path);

    }
}
else
{
    if (Directory.Exists(input_path))
    {
        files_to_read.AddRange(Directory.GetFiles(input_path));
    }
    else if (File.Exists(input_path)) files_to_read.Add(input_path);
}

Console.WriteLine(string.Join(',', files_to_read));

foreach (var path in files_to_read)
{
    new Grammar(path)
        .SaveTxt("./output/1_Bruto/")
        .SaveXlsx("./output/1_Bruto/")
    .Use(new RemoveEmpty())
        .SaveTxt("./output/2_RemVazio/")
        .SaveXlsx("./output/2_RemVazio/")
    .Use(new RemoveNonDeterministic())
        .SaveTxt("./output/3_RemNonDet/")
        .SaveXlsx("./output/3_RemNonDet/")
    .Use(new RemoveNonAcessible())
        .SaveTxt("./output/4_RemNonAcess/")
        .SaveXlsx("./output/4_RemNonAcess/")
    .Use(new RemoveEquivalent())
        .SaveTxt("./output/5_Otimo/")
        .SaveXlsx("./output/5_Otimo/")
        .SaveRules("./output/5_Otimo/")
    ;
}