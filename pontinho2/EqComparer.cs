using System;
using System.Collections.Generic;
using System.Linq;
public class MyEqualityComparer : IEqualityComparer<string[]>
{
    public bool Equals(string[] x, string[] y)
    {
        x = x.OrderBy(k => k).ToArray();
        y = y.OrderBy(k => k).ToArray();

        if (x.Length != y.Length)
        {
            return false;
        }
        for (int i = 0; i < x.Length; i++)
        {
            if (x[i] != y[i])
            {
                return false;
            }
        }
        return true;
    }

    public int GetHashCode(string[] obj)
    {
        int result = 17;
        for (int i = 0; i < obj.Length; i++)
        {
            unchecked
            {
                result = result * 23 + obj[i].Sum(c => Convert.ToInt32(c));
            }
        }
        return result;
    }
}