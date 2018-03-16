using System.Collections.Generic;

namespace Dissect.Parser.LALR1.Analysis
{
    public class ActionAndGoTo
    {
        public Dictionary<int, Dictionary<string, int>> Action = new Dictionary<int, Dictionary<string, int>>();
        public Dictionary<int, Dictionary<string, int>> GoTo = new Dictionary<int, Dictionary<string, int>>();
        private static void Add(Dictionary<int, Dictionary<string, int>> dict, int num, string trigger, int destination)
        {
            if (!dict.ContainsKey(num))
            {
                dict.Add(num, new Dictionary<string, int>());
            }
            dict[num][trigger] = destination;
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
