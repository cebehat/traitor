using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cebt.Shared
{
    public static class Extensions
    {
        public static bool In<T>(this T val, params T[] values) where T : struct
        {
            return values.Contains(val);
        }

        public static bool OneGreaterOrLessThan(this int val, int val2)
        {
            return (val + 1) == val2 || (val - 1) == val2;
        }
    } 
}
