namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall
{
    interface ITES4Callable
    {
        TES4Function Function { get; }
        TES4ApiToken? CalledOn { get; }
    }
}