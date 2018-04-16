using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Other
{
    public static class ActorValueMap
    {
        public static readonly Dictionary<string, string> Map = new Dictionary<string, string>()
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
    }
}
