using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Property
{//Copied to Skyblivion.Clients
    internal static class TES5PropertySorter
    {
        private static readonly Regex
            scriptHeaderAndDeclarationsRegex = new Regex("(?<=^ScriptName [^\r\n]+\r\n)(?<properties>.*?)(?=\r\n(Function|Event|State|Auto State) )", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled),
            scriptFooterAndDeclarationsRegex = new Regex("(?<=;END FRAGMENT CODE - Do not edit anything between this and the begin comment(\r\n)+)(?<properties>.+)", RegexOptions.Singleline | RegexOptions.Compiled),
            declarationRegex = new Regex(@"^[ \t]*(?<dataType>[A-Za-z0-9_]+(\[\])?)(?<property> +Property)? +(?<name>[A-Za-z0-9_]+)(?<initialization>[ \t]*=[ \t]*[^ ;\r\n]+)?(?<attributes>[ \t]+[^;\r\n]+)?(?<comment>;[^\r\n]+)?", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase),
            autoRegex = new Regex(@"\bAuto\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            conditionalRegex = new Regex(@"\bConditional\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            tes4FormIDRegex = new Regex(";TES4FormID:[0-9]+;"),
            tes5FormIDRegex = new Regex(";TES5FormID:[0-9]+;");
        private static readonly string[] conditionalTypes = new string[] { "Bool", "Int", "Float" };
        private static string SortPropertyBlock(string propertyBlock, bool allowNonAuto, out bool found)
        {
            if (propertyBlock.Split(["\r\n"], StringSplitOptions.None).Any(l => l.StartsWith(";") || l.StartsWith("{"))) { found = false; return propertyBlock; }//Contains comments
            if (propertyBlock.IndexOf("Property", StringComparison.OrdinalIgnoreCase) == -1) { found = false; return propertyBlock; }//Contains no properties
            var matches = declarationRegex.Matches(propertyBlock).Cast<Match>();
            var propertyMatches = matches.Where(m => m.Groups["property"].Success).ToArray();
            var fieldMatches = matches.Where(m => !m.Groups["property"].Success).ToArray();
            if (!allowNonAuto && propertyMatches.Where(m => !autoRegex.IsMatch(m.Groups["attributes"].Value)).Any())
            {
                throw new InvalidOperationException("Some properties did not have \"Auto\"");
            }
            if (propertyMatches.Length + fieldMatches.Length != propertyBlock.Split(["\r\n"], StringSplitOptions.RemoveEmptyEntries).Where(s => !string.IsNullOrWhiteSpace(s)).Count())
            {
                if (propertyBlock.Contains("Leveled Vampire Actors Easier to replace with ReferenceAliases")) { found = false; return propertyBlock; }//TES4MG13Script
                if (propertyBlock.Contains("Float Function min(Float x, Float y) global ;min function for sell price")) { found = false; return propertyBlock; }//TES4TGFenceChestScript
                if (propertyBlock.Contains("Int Property PitDogRank = 0 AutoReadOnly")) { found = false; return propertyBlock; }//TES4ArenaMatchControlScript (The Regex didn't understand this script, but I don't want to reforamt it anyway.)
                if (propertyBlock.Contains("Function CaptureVars()")) { found = false; return propertyBlock; }//TES4_QF_TES4ArenaMatch_0202E8D6
                throw new InvalidOperationException("Properties were not correctly recognized.");
            }
            var properties = matches.Select(m => new { DataType = m.Groups["dataType"].Value, IsProperty = m.Groups["property"].Success, PropertyName = m.Groups["name"].Value, Initialization = m.Groups["initialization"].Value, Attributes = m.Groups["attributes"].Value, Comment = m.Groups["comment"].Value }).ToArray();
            var groupedProperties = properties
                .GroupBy(p => p.DataType)
                .OrderBy(g => g.Key)
                .ToArray();
            found = true;
            return string.Join("\r\n", groupedProperties.SelectMany((g, i) =>
            {
                int propertyNameMaxLength = g.Select(p => p.PropertyName.Length).Max();
                var groupEnumerable = g.OrderBy(p => p.PropertyName)
                    .Select(p =>
                    {
                        string initialization = p.Initialization != "" ? " = " + p.Initialization.Trim().Substring(1).Trim() : "";
                        string attributes = p.Attributes;
                        bool canBeConditional = conditionalTypes.Any(t => t.Equals(p.DataType, StringComparison.OrdinalIgnoreCase));
                        //bool wasAuto = autoRegex.IsMatch(attributes);
                        //bool wasConditional = conditionalRegex.IsMatch(attributes);
                        //bool shouldBeConditional = p.IsProperty && wasAuto && canBeConditional;
                        //if (!wasConditional && shouldBeConditional) { attributes += " Conditional"; }
                        if (!canBeConditional) { attributes = conditionalRegex.Replace(attributes, ""); }
                        if (attributes != "") { attributes = " " + attributes.Trim(); }
                        string comment = p.Comment;
                        comment = tes4FormIDRegex.Replace(comment, "");
                        comment = tes5FormIDRegex.Replace(comment, "");
                        comment = comment.Trim();
                        if (comment.All(c => c == ';')) { comment = ""; }
                        if (comment != "") { comment = " ; " + comment.Substring(1).Trim(); }
                        string afterPropertyName = initialization + attributes + comment;
                        return p.DataType + (p.IsProperty ? " Property" : "") + " " + (afterPropertyName != "" ? p.PropertyName.PadRight(propertyNameMaxLength) + afterPropertyName : p.PropertyName);
                    });
                if (i < groupedProperties.Length - 1) { groupEnumerable = groupEnumerable.Concat(new string[] { "" }); }
                return groupEnumerable;
            }));
        }

        private static string ReplaceWithinScriptHeader(string psc, bool allowNonAuto, out bool found)
        {
            bool foundLocal = false;
            string replaced = scriptHeaderAndDeclarationsRegex.Replace(psc, m =>
            {
                string unsorted = m.Groups["properties"].Value;
                string sorted = SortPropertyBlock(unsorted, allowNonAuto, out foundLocal);
                return foundLocal ? "\r\n" + sorted.Trim() + "\r\n" : unsorted;
            });
            found = foundLocal;
            return replaced;
        }

        private static string ReplaceWithinScriptFooter(string psc)
        {
            return scriptFooterAndDeclarationsRegex.Replace(psc, m =>
            {
                bool found;
                string unsorted = m.Groups["properties"].Value;
                string sorted = SortPropertyBlock(unsorted, true, out found);
                return found ? "\r\n" + sorted.Trim() : unsorted;
            });
        }

        public static string ReplaceWithinHeaderOrFooter(string psc)
        {
            bool found;
            string replaced = ReplaceWithinScriptHeader(psc, true, out found);
            if (found) { return replaced; }
            return ReplaceWithinScriptFooter(psc);
        }

        public static string? ExtractAndSortProperties(string psc)
        {
            var match = scriptHeaderAndDeclarationsRegex.Match(psc);
            return match.Success ? SortPropertyBlock(match.Groups["properties"].Value, false, out _) : null;
        }
    }
}
