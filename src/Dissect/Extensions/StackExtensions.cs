using System.Collections.Generic;

namespace Dissect.Extensions
{
    public static class StackExtensions
    {
        public static IEnumerable<T> Pop<T>(this Stack<T> stack, int count)
        {
            int popped = 0;
            while(popped<count)
            {
                yield return stack.Pop();
                popped++;
            }
        }
    }
}