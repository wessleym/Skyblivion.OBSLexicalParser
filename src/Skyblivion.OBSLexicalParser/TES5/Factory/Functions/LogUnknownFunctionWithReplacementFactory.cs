using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    internal class LogUnknownFunctionWithReplacementFactory : IFunctionFactory
    {
        private readonly LogUnknownFunctionFactory logUnknownFunctionFactory;
        private readonly ITES5ValueCodeChunk replacement;
        public LogUnknownFunctionWithReplacementFactory(LogUnknownFunctionFactory logUnknownFunctionFactory, ITES5ValueCodeChunk replacement)
        {
            this.logUnknownFunctionFactory = logUnknownFunctionFactory;
            this.replacement = replacement;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            return logUnknownFunctionFactory.CreateReplacement(function, replacement, codeScope);
        }
    }
}
