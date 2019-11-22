using System.Collections.Generic;
using System.Linq;

namespace Dissect.Parser.LALR1.Analysis.KernelSet
{
    /*
     * A BST implementation for more efficient lookup
     * of states by their kernel items.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    class KernelSet
    {
        protected int NextNumber = 0;
        protected Node? Root = null;
        /*
        * Inserts a new node in the BST and returns
        * the number of the new state if no such state
        * exists. Otherwise, returns the number of the
        * existing state.
        *
        *  The state kernel.
        *
        *  The state number.
        */
        public int Insert(IList<decimal[]> kernelArg)
        {
            decimal[] kernel = HashKernel(kernelArg);
            if (this.Root == null)
            {
                int n = this.NextNumber++;
                this.Root = new Node(kernel, n);
                return n;
            }

            Node node = this.Root;
            while (true)
            {
                if (ArrayLessThan(kernel, node.Kernel))
                {
                    if (node.Left == null)
                    {
                        int n = this.NextNumber++;
                        node.Left = new Node(kernel, n);
                        return n;
                    }
                    else
                    {
                        node = node.Left;
                    }
                }
                else if (ArrayGreaterThan(kernel, node.Kernel))
                {
                    if (node.Right == null)
                    {
                        int n = this.NextNumber++;
                        node.Right = new Node(kernel, n);
                        return n;
                    }
                    else
                    {
                        node = node.Right;
                    }
                }
                else
                {
                    return node.Number;
                }
            }
        }

        /*
        * Hashes a state kernel using a pairing function.
         *
         * @param array kernel The kernel.
         *
         * @return array The hashed kernel.
        */
        public static decimal[] HashKernel(IList<decimal[]> kernel)
        {
            return kernel.Select(k =>
            {
                decimal car = k[0];
                decimal cdr = k[1];
                return (car + cdr) * (car + cdr + 1) / 2 + cdr;
            }).OrderBy(k => k).ToArray();
        }

        private static bool ArrayGreaterThan(decimal[] left, decimal[] right)//WTM:  Change:  In PHP, arrays were compared like this:  array > array.  Apparently this is equivalent to comparing lengths and then comparing each item.
        {
            if (left.Length != right.Length) { return left.Length > right.Length; }
            for (int i = 0; i < left.Length; i++)
            {
                if (left[i] > right[i]) { return true; }
            }
            return false;
        }

        private static bool ArrayLessThan(decimal[] left, decimal[] right)//WTM:  Change:  In PHP, arrays were compared like this:  array < array.  Apparently this is equivalent to comparing lengths and then comparing each item.
        {
            if (left.Length != right.Length) { return left.Length < right.Length; }
            for (int i = 0; i < left.Length; i++)
            {
                if (left[i] < right[i]) { return true; }
            }
            return false;
        }
    }
}