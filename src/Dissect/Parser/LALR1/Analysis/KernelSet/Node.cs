namespace Dissect.Parser.LALR1.Analysis.KernelSet
{
    class Node
    {
        public readonly decimal[] Kernel;
        public readonly int Number;
        public Node? Left = null;
        public Node? Right = null;
        public Node(decimal[] hashedKernel, int number)
        {
            this.Kernel = hashedKernel;
            this.Number = number;
        }
    }
}