using Dissect.Extensions;
using System.Collections.Generic;

namespace Dissect.Parser.LALR1.Analysis
{
    public class ActionAndGoTo
    {
        public readonly Dictionary<int, Dictionary<string, int>> Action = new Dictionary<int, Dictionary<string, int>>();
        public readonly Dictionary<int, Dictionary<string, int>> GoTo = new Dictionary<int, Dictionary<string, int>>();
        private static void Add(Dictionary<int, Dictionary<string, int>> dict, int num, string trigger, int destination)
        {
            Dictionary<string, int> innerDict = dict.GetOrAdd(num, () => new Dictionary<string, int>());
            innerDict.Add(trigger, destination);
        }
        public void AddAction(int num, string trigger, int destination)
        {
            Add(Action, num, trigger, destination);
        }
        public void AddGoTo(int num, string trigger, int destination)
        {
            Add(GoTo, num, trigger, destination);
        }
    }
}
