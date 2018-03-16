using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dissect.Util
{
    /*
     * Some utility functions.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    static class Util
    {
        /*
        * Merges two or more sets by values.
         *
         * {a, b} union {b, c} = {a, b, c}
        */
        public static IEnumerable<T> union<T>(params IEnumerable<T>[] args)
        {
            IEnumerable<T> rv = new T[] { };
            foreach(IEnumerable<T> a in args)
            {
                rv = rv.Concat(a);
            }
            return rv.Distinct();
        }

        /*
        * Determines whether two sets have a difference.
        */
        public static bool different<T>(IList<T> first, IList<T> second)
        {
            return first.Where(f=>!second.Contains(f)).Any();//WTM:  This was previously implemented backward:  count(array_diff(first, second)) == 0;
        }

        /*
        * Determines length of a UTF-8 string.
        */
        private static Lazy<Encoding> iso_8859_1 = new Lazy<Encoding>(() => Encoding.GetEncoding("iso-8859-1"));
        public static int stringLength(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            return iso_8859_1.Value.GetString(bytes, 0, bytes.Length).Length;
        }

        /*
        * Extracts a substring of a UTF-8 string.
        */
        public static string substring(string str, int position, Nullable<int> length = null)
        {
            if (length == null)
            {
                length = stringLength(str);
            }

            return str.Substring(position, length.Value);
        }
    }
}
 