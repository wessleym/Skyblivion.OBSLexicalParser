using Skyblivion.OBSLexicalParser.TES4.AST.Code;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value
{
    interface ITES4Value : ITES4CodeFilterable
    {
        /*
        * Get the value representation
        */
        object Data { get; }

        /*
        * Is this value related to execution or not?
        */
        bool HasFixedValue { get; }
    }
}