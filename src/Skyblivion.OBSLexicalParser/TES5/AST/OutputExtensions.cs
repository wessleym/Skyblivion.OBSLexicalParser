using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST
{
    internal static class OutputExtensions
    {
        public static IEnumerable<string> Indent(this IEnumerable<string> output)
        {
            return output.Select(o => TES5Script.WithIndent(o));
        }
        public static IEnumerable<string> SkipEmptyStartAndIndent(this IEnumerable<string> output)
        {
            return output.Where((o, i) => TES5Script.AvoidEmptyStart(o, i)).Indent();
        }
    }
}
