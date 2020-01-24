using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    /*
     * Class FillerFactory
     */

    //WTM:  Change:  Aerisarn requested that this factory be replaced with a logging factory (LogUnknownFunctionFactory).
    //I'm going to let addachievement keep using this factory since Oblivion achievements don't apply in Skyrim's system.
    class FillerFactory : IFunctionFactory
    {
        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            return new TES5Filler();
        }
    }
}