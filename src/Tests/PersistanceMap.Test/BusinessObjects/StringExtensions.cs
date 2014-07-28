using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.Test
{
    public static class StringExtensions
    {
        public static string Flatten(this string value)
        {
            return value
                .Replace("\t", "")
                .Replace("\n", "")
                .Replace("\r", "");
        }
    }
}
