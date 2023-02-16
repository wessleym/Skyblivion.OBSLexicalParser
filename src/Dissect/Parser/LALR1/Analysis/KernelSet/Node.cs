namespace Dissect.Parser.LALR1.Analysis.KernelSet
{
    public class Node
    {
        public decimal[] Kernel { get; }
        public int Number { get; }
        public Node? Left{ get; internal set; }
        public Node? Right { get; internal set; }
        public Node(decimal[] hashedKernel, int number)
        {
            this.Kernel = hashedKernel;
            this.Number = number;
        }
    }
}