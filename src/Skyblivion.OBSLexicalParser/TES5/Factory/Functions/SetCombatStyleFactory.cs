using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SetCombatStyleFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        private readonly LogUnknownFunctionFactory logUnknownFunctionFactory;
        public SetCombatStyleFactory(TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, LogUnknownFunctionFactory logUnknownFunctionFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.logUnknownFunctionFactory = logUnknownFunctionFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            const string functionName = "SetCombatStyle";
            TES4FunctionArguments functionArguments = function.Arguments;
            if (functionArguments.Count == 0) { return logUnknownFunctionFactory.ConvertFunction(calledOn, function, codeScope, globalScope, multipleScriptsScope); }
            TES5ObjectCall getActorBase = this.objectCallFactory.CreateGetActorBase(calledOn);
            TES5ObjectCallArguments newArguments = this.objectCallArgumentsFactory.CreateArgumentList(functionArguments, codeScope, globalScope, multipleScriptsScope);
            return this.objectCallFactory.CreateObjectCall(getActorBase, functionName, newArguments);
        }
    }
}
