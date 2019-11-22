using Skyblivion.OBSLexicalParser.TES5.AST.Property;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    /*
     * Interface ITES5Referencer
     *
     * Implementers declare that are referencing something.
     */
    interface ITES5Referencer : ITES5ValueCodeChunk
    {
        /*
        * Returns the thing this references to LOCALLY. REMOTE references are considered null.
        */
        ITES5VariableOrProperty? ReferencesTo { get; }
        string Name { get; }
    }
}