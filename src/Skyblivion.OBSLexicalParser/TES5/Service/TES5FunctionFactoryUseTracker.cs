using Skyblivion.ESReader.Extensions.IDictionaryExtensions;
using Skyblivion.OBSLexicalParser.TES5.Factory.Functions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Service
{
    static class TES5FunctionFactoryUseTracker
    {
        private static readonly Dictionary<Tuple<string, Type>, List<string>> dictionary = new Dictionary<Tuple<string, Type>, List<string>>();
        public static void Add(string functionName, IFunctionFactory functionFactory, string scriptName)
        {
            dictionary.AddNewListIfNotContainsKeyAndAddValueToList(new Tuple<string, Type>(functionName.ToLower(), functionFactory.GetType()), scriptName.ToLower());
        }

        public static void WriteTableOfUnknownFunctions()
        {
            Type fillerFactoryType = typeof(FillerFactory);
            Type logUnknownFunctionFactoryType = typeof(LogUnknownFunctionFactory);
            int column1 = dictionary.Select(kvp => kvp.Key.Item1.Length).Max();
            int column2 = ("Invocation Count").Length;
            Console.WriteLine(("Function Name").PadRight(column1) + ("Invocation Count").PadLeft(column2));
            var fillerFactoryCalls = dictionary.Where(kvp => kvp.Key.Item2 == fillerFactoryType || kvp.Key.Item2 == logUnknownFunctionFactoryType).ToArray();
            foreach (var kvp in fillerFactoryCalls.OrderByDescending(kvp => kvp.Value.Count).ThenBy(kvp => kvp.Key.Item1))
            {
                Console.WriteLine(kvp.Key.Item1.PadRight(column1) + kvp.Value.Count.ToString().PadLeft(column2));
            }
            Console.WriteLine();
            Console.WriteLine(("Function Name").PadRight(column1) + ("Script Count").PadLeft(column2));
            foreach (var kvp in fillerFactoryCalls.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Distinct().Count()).OrderByDescending(kvp => kvp.Value).ThenBy(kvp => kvp.Key.Item1))
            {
                Console.WriteLine(kvp.Key.Item1.PadRight(column1) + kvp.Value.ToString().PadLeft(column2));
            }
        }
    }
}
