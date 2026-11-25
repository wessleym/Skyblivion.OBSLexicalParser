using Dissect.Extensions;
using Skyblivion.OBSLexicalParser.TES5.Factory.Functions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Service
{
    static class TES5FunctionFactoryUseTracker
    {
        private static readonly Dictionary<string, List<Tuple<Type, string>>> dictionary = new Dictionary<string, List<Tuple<Type, string>>>(StringComparer.OrdinalIgnoreCase);
        public static void Add(string functionName, IFunctionFactory functionFactory, string scriptName)
        {
            dictionary.AddNewListIfNotContainsKeyAndAddValueToList(functionName, new Tuple<Type, string>(functionFactory.GetType(), scriptName));
        }

        public static void WriteTableOfUnknownFunctions()
        {
            Type[] soughtTypes = new Type[] { typeof(LogUnknownFunctionFactory) };
            int column1 = dictionary.Select(kvp => kvp.Key.Length).Max();
            int column2 = ("Invocation Count").Length;
            Console.WriteLine(("Function Name").PadRight(column1) + ("Invocation Count").PadLeft(column2));
            var reducedDictionary = dictionary
                .Where(kvp => !kvp.Key.Equals("istimepassing", StringComparison.OrdinalIgnoreCase) && !kvp.Key.Equals("isxbox", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Where(kvp2 => soughtTypes.Contains(kvp2.Item1)).ToArray())
                .Where(kvp => kvp.Value.Length > 0)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            foreach (var kvp in reducedDictionary
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Length)
                .OrderByDescending(kvp => kvp.Value)
                .ThenBy(kvp => kvp.Key, StringComparer.OrdinalIgnoreCase))
            {
                Console.WriteLine(kvp.Key.PadRight(column1) + kvp.Value.ToString().PadLeft(column2));
            }
            Console.WriteLine();
            Console.WriteLine(("Function Name").PadRight(column1) + ("Script Count").PadLeft(column2));
            foreach (var kvp in reducedDictionary
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.DistinctBy(kvp2 => kvp2.Item2).Count())
                .OrderByDescending(kvp => kvp.Value)
                .ThenBy(kvp => kvp.Key, StringComparer.OrdinalIgnoreCase))
            {
                Console.WriteLine(kvp.Key.PadRight(column1) + kvp.Value.ToString().PadLeft(column2));
            }
        }
    }
}
