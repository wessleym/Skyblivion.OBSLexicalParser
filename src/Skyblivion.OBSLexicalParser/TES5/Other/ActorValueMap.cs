using System;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Other
{
    public static class ActorValueMap
    {
        private static readonly Dictionary<string, string> map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "fatigue", "Stamina" },
            { "armorer", "Smithing" },
            { "security", "Lockpicking" },
            { "acrobatics", "Sneak" },
            { "mercantile", "Speechcraft" },
            { "mysticism", "Illusion" },
            { "blade", "OneHanded" },
            { "blunt", "OneHanded" },
            { "encumbrance", "InventoryWeight" },
            { "spellabsorbchance", "AbsorbChance" },
            { "resistfire", "FireResist" },
            { "resistfrost", "FrostResist" },
            { "resistdisease", "DiseaseResist" },
            { "resistmagic", "MagicResist" },
            { "resistpoison", "PoisonResist" },
            { "resistshock", "ElectricResist" }
        };

        public static string GetRevisedName(string actorValueName)
        {
            string? mappedArgument;
            return map.TryGetValue(actorValueName, out mappedArgument) ? mappedArgument : actorValueName;
        }
    }
}
