using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    class TES5Castable
    {
        /*
        * Used only for Float . int cast
        * Hacky. Should be removed at some point.
        */
        public ITES5Type? ManualCastTo = null;

        public string ManualCastToOutput => ManualCastTo != null ? " as " + ManualCastTo.Value : "";
    }
}
