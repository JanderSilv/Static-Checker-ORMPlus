using System;
using static System.Console;
using System.IO;
using System.Collections.Generic;

Reader r = new Reader();
Atom current = new Atom();

current.Read(r);
int c = 1;
int l = 0;
current.Count(0, ref c, ref l);
WriteLine(current.ToString());

