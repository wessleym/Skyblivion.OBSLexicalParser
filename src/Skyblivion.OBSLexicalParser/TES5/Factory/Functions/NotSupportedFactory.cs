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
                //WTM:  Note:  Oddly, this project can handle GetAmountSoldStolen, even though that's not a Papyrus function.  But it is an SKSE function, as is ModAmountSoldStolen:
                //http://skse.silverlock.org/vanilla_commands.html
                //Maybe SKSE's function can be used some day.
                functionName.Equals("ModAmountSoldStolen", StringComparison.OrdinalIgnoreCase) ? "Skyrim doesn't track how much gold was earned with stolen goods." :
                //WTM:  Note:  GetStartingPos is treated as a special case in TES5ValueFactory.  If the below occurs, the special case didn't catch it.
                functionName.Equals("GetStartingPos", StringComparison.OrdinalIgnoreCase) ? "GetStartingPos isn't directly convertable." :
                null;
            string message = "Function " + functionName + " not supported." + (reason != null ? "  " + reason : "");
            throw new ConversionException(message, expected: true);
        }
    }
}