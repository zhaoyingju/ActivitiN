using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bpmn_converter.converter.util
{
    public static class StringExtensionMethods
    {
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static bool equalsIgnoreCase(this string text, string txtDiff)
        {
            return text.Equals(txtDiff, StringComparison.InvariantCultureIgnoreCase);
        }

        public static int length(this string text)
        {
            return text.Length;
        }

    }
}
