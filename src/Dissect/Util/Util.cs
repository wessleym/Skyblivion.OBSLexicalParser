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
            return first.Where(f=>!second.Contains(f)).Any();
        }

        /*
        * Determines length of a UTF-8 string.
        */
        public static int stringLength(string str)
        {
            return str.Length;
        }

        /*
        * Extracts a substring of a UTF-8 string.
        */
        public static string substring(string str, int position, Nullable<int> length = null)
        {
            if (length == null)
            {
                return str.Substring(position);
            }
            return str.Substring(position, length.Value);
        }
    }
}
 