using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    interface IFunctionFactory
    {
        /*
        * Convert a function from TES4 to TES5.
        *  The reference upon which call is done ( Given A.B(), calledOn = A )
        *  The function called ( Given A.B() , function = B )
        *  Code ( branch ) scope we"re in
        *  Script scope we"re in
        *  Container of all scripts compiled together
        */
        ITES5CodeChunk convertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope);
    }
}