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
        protected int nextNumber = 0;
        protected Node root = null;
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
        public int insert(IList<decimal[]> kernelArg)
        {
            decimal[] kernel = hashKernel(kernelArg);
            if (this.root == null)
            {
                int n = this.nextNumber++;
                this.root = new Node(kernel, n);
                return n;
            }

            Node node = this.root;
            while (true)
            {
                if (kernel.Length < node.kernel.Length)//WTM:  Change:  In PHP, this was kernal < node.kernel (array < array).  Apparently this is equivalent to comparing lengths.
                {
                    if (node.left == null)
                    {
                        int n = this.nextNumber++;
                        node.left = new Node(kernel, n);
                        return n;
                    }
                    else
                    {
                        node = node.left;
                    }
                }
                else if(kernel.Length > node.kernel.Length)//WTM:  Change:  In PHP, this was kernal > node.kernel (array > array).  Apparently this is equivalent to comparing lengths.
                {
                    if (node.right == null)
                    {
                        int n = this.nextNumber++;
                        node.right = new Node(kernel, n);
                        return n;
                    }
                    else
                    {
                        node = node.right;
                    }
                }
                else 
                {
                    return node.number;
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
        public static decimal[] hashKernel(IList<decimal[]> kernel)
        {
            return kernel.Select(k =>
            {
                decimal car = k[0];
                decimal cdr = k[1];
                return (car + cdr) * (car + cdr + 1) / 2 + cdr;
            }).OrderBy(k => k).ToArray();
        }
    }
}