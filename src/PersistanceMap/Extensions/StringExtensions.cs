using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap
{
    internal static class StringExtensions
    {
        public static string RemoveLineBreak(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            while (str.EndsWith("\r\n"))
            {
                str = str.Substring(0, str.Length - 2);
            }

            return str;
        }
    }
}
