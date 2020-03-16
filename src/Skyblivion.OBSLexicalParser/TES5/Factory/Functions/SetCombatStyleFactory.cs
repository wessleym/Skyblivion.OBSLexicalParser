using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SetCombatStyleFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly DefaultFunctionFactory defaultFunctionFactory;
        public SetCombatStyleFactory(TES5ObjectCallFactory objectCallFactory, DefaultFunctionFactory defaultFunctionFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.defaultFunctionFactory = defaultFunctionFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            return defaultFunctionFactory.ConvertFunction(this.objectCallFactory.CreateGetActorBase(calledOn), function, codeScope, globalScope, multipleScriptsScope);
        }
    }
}
