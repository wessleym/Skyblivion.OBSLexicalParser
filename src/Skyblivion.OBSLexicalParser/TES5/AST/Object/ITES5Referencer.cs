using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    /*
     * Interface ITES5Referencer
     * @package Ormin\OBSLexicalParser\TES5\AST\Object
     *
     * Implementers declare that you can reference it.
     */
    interface ITES5Referencer : ITES5Value, ITES5CodeChunk//WTM:  Change:  Added ITES5CodeChunk.
    {
        /*
        * Returns the thing this references to LOCALLY. REMOTE references are considered null.
        */
        ITES5Variable getReferencesTo();
        string getName();
    }
}