using Dissect.Extensions;
using Dissect.Parser.LALR1.Analysis.KernelSet;
using System.Collections.Generic;

namespace Dissect.Parser.LALR1.Analysis
{
    public class ActionAndGoTo
    {
        public readonly Dictionary<int, Dictionary<string, int>> Action = new Dictionary<int, Dictionary<string, int>>();
        public readonly Dictionary<int, Dictionary<string, int>> GoTo = new Dictionary<int, Dictionary<string, int>>();
        private static void Add(Dictionary<int, Dictionary<string, int>> dict, Node source, string trigger, Node destination)
        {
            Dictionary<string, int> innerDict = dict.GetOrAdd(source.Number, () => new Dictionary<string, int>());
            innerDict.Add(trigger, destination.Number);
        }
        public void AddAction(Node source, string trigger, Node destination)
        {
            Add(Action, source, trigger, destination);
        }
        public void AddGoTo(Node source, string trigger, Node destination)
        {
            Add(GoTo, source, trigger, destination);
        }
    }
}
