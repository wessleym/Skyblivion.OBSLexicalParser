using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    class TES5PlayerReference : ITES5Referencer
    {
        public IEnumerable<string> Output => new string[] { "Game.getPlayer()" }; //pretty ugly to do it here.

        public string Name => "player";

        public ITES5Variable ReferencesTo => null;

        public ITES5Type TES5Type => TES5BasicType.T_ACTOR;
    }
}