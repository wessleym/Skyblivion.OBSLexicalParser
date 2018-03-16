namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall
{
    interface ITES4Callable
    {
        TES4Function getFunction();
        TES4ApiToken getCalledOn();
    }
}