using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Value
{
    /*
     * Interface TES5Value
     *
     * Represents something that returns a result value upon evaluating. This might be a primitive, an expression, an data-returning object call, or an reference to a property.
     */
    interface ITES5Value : ITES5Outputtable
    {
        ITES5Type TES5Type { get; }
    }
}