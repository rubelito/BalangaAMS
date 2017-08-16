using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalangaAMS.Core.HelperDomain
{
    public static class StringExtensions
    {
        public static bool ContainsAny(this string str, string searchstr)
        {
            string[] listofvalue = searchstr.Split(' ');

            if (!string.IsNullOrEmpty(str) || listofvalue.Length > 0)
            {
                foreach (string value in listofvalue)
                {
                    if (str.Contains(value))
                        return true;
                }
            }
            return false;
        }
    }
}
