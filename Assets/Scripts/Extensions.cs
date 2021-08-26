using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class Extensions
{
    public static bool In<T>(this T val, params T[] values) where T : struct
    {
        return values.Contains(val);
    }
}
