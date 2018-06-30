using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class NotSupportedFactory : IFunctionFactory
    {
        public ITES5ValueCodeChunk convertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            string functionName = function.FunctionCall.FunctionName;
            string reason =
                functionName.Equals("GetIsPlayerBirthsign", StringComparison.OrdinalIgnoreCase) ? "Skyrim doesn't have birthsigns--only standing stones." :
                null;
            string message = "Function " + functionName + " not supported." + (reason != null ? "  " + reason : "");
            throw new ConversionException(message, expected: true);
        }
    }
}