using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    class TES5PlayerReference : ITES5Referencer
    {
        public List<string> output()
        {
            return new List<string>() { "Game.getPlayer()" }; //pretty ugly to do it here.
        }

        public string getName()
        {
            return "player";
        }

        public ITES5Variable getReferencesTo()
        {
            return null;
        }

        public ITES5Type getType()
        {
            return TES5BasicType.T_ACTOR;
        }
    }
}