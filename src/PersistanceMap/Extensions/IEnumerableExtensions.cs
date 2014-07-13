using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PersistanceMap
{
    public static class IEnumerableExtensions
    {
        [DebuggerStepThrough]
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (var value in enumeration)
                action(value);
        }

        public static T[] ToArray<T>(this IEnumerable<T> enumeration)
        {
            if (enumeration == null)
                return null;

            long size = enumeration.Count();
            T[] tArray = new T[size];
            int i = 0;
            enumeration.ForEach(itm =>
            {
                tArray[i] = itm;
                i++;
            });

            return tArray;
        }
    }
}
