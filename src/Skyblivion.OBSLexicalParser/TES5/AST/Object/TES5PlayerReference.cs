using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    class TES5PlayerReference : ITES5Referencer
    {
        private const string playerName = "player";

        public const string PlayerRefName = "PlayerRef";

        public IEnumerable<string> Output => new string[] { PlayerRefName };

        public string Name => playerName;

        public ITES5VariableOrProperty? ReferencesTo => null;

        public ITES5Type TES5Type => TES5TypeStatic;

        public static readonly TES5BasicType TES5TypeStatic = TES5BasicType.T_ACTOR;

        public const int FormID = 0x14;

        public static bool EqualsPlayer(string name)
        {
            return name.Equals(playerName, StringComparison.OrdinalIgnoreCase);
        }
    }
}