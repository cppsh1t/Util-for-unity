using System.Collections.Generic;
using UnityEngine;



public static class DumpExtension
{
    public static void Dump(this string self)
    {
        Debug.Log(self);
    }

    public static void Dump(this object self)
    {
        Debug.Log(self);
    }

    public static void Dump(this IEnumerable<object> self)
    {
        Debug.Log($"[{string.Join(", ", self)}]");
    }


    public static void Dump(this IEnumerable<int> self)
    {
        Debug.Log($"[{string.Join(", ", self)}]");
    }

    public static void Dump(this IEnumerable<float> self)
    {
        Debug.Log($"[{string.Join(", ", self)}]");
    }

    public static void Dump(this IEnumerable<double> self)
    {
        Debug.Log($"[{string.Join(", ", self)}]");
    }

    public static void Dump(this IEnumerable<long> self)
    {
        Debug.Log($"[{string.Join(", ", self)}]");
    }

    public static void Dump(this IEnumerable<short> self)
    {
        Debug.Log($"[{string.Join(", ", self)}]");
    }

    public static void Dump(this IEnumerable<byte> self)
    {
        Debug.Log($"[{string.Join(", ", self)}]");
    }

    public static void Dump(this IEnumerable<bool> self)
    {
        Debug.Log($"[{string.Join(", ", self)}]");
    }

    public static void Dump(this IEnumerable<uint> self)
    {
        Debug.Log($"[{string.Join(", ", self)}]");
    }

    public static void Dump(this IEnumerable<ulong> self)
    {
        $"[{string.Join(", ", self)}]".Dump();
    }

    public static void Dump(this IEnumerable<ushort> self)
    {
        $"[{string.Join(", ", self)}]".Dump();
    }

    public static void Dump(this IEnumerable<decimal> self)
    {
        $"[{string.Join(", ", self)}]".Dump();
    }

    public static void Dump<T, R>(this IDictionary<T, R> self) 
    {
        $"[{string.Join(", ", self)}]".Dump();
    }
}