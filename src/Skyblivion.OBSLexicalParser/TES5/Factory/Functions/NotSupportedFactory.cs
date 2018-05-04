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
                functionName.Equals("ForceFlee", StringComparison.OrdinalIgnoreCase) ? "Papyrus doesn't seem to be able to force NPCs to flee." :
                functionName.Equals("GetIsPlayerBirthsign", StringComparison.OrdinalIgnoreCase) ? "Skyrim doesn't have birthsigns--only standing stones." :
                functionName.Equals("GetStartingPos", StringComparison.OrdinalIgnoreCase) ? "Papyrus doesn't seem to know the original location of items (before they were moved)." :
                functionName.Equals("ModAmountSoldStolen", StringComparison.OrdinalIgnoreCase) ? "Skyrim doesn't track how much gold was earned with stolen goods." :
                functionName.Equals("SetLevel", StringComparison.OrdinalIgnoreCase) ? "SKSE has Game.SetPlayerLevel(int level), but Oblivion calls this function on other actors." :
                null;
            string message = "Function " + functionName + " not supported." + (reason != null ? "  " + reason : "");
            throw new ConversionException(message, expected: true);
        }
    }
}