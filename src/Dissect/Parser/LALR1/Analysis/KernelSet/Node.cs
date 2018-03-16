namespace Dissect.Parser.LALR1.Analysis.KernelSet
{
    class Node
    {
        public decimal[] kernel;
        public int number;
        public Node left = null;
        public Node right = null;
        public Node(decimal[] hashedKernel, int number)
        {
            this.kernel = hashedKernel;
            this.number = number;
        }
    }
}