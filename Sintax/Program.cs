using System;

if (args.Length < 1)
{
    Console.WriteLine("Caminho de arquivo não informado.");
    return;
}


for (int i = 0; i < args.Length; i++)
{
    Sintax s = new(args[i]);
}

