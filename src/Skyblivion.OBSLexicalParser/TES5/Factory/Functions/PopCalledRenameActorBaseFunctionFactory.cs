using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class PopCalledRenameActorBaseFunctionFactory : IFunctionFactory
    {
        private readonly string newFunctionName;
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public PopCalledRenameActorBaseFunctionFactory(string newFunctionName, TES5ReferenceFactory referenceFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory)
        {
            this.newFunctionName = newFunctionName;
            this.referenceFactory = referenceFactory;
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4FunctionArguments functionArguments = function.Arguments;
            string newCalledOnString = functionArguments.Pop(0).StringValue;
            ITES5Referencer newCalledOn = this.referenceFactory.CreateReadReference(newCalledOnString, globalScope, multipleScriptsScope, localScope);
            if (TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(newCalledOn.TES5Type, TES5BasicType.T_ACTOR)) { newCalledOn = this.objectCallFactory.CreateGetActorBase(newCalledOn); }
            else
            {
                throw new ConversionException(newFunctionName + " should be called with an Actor.  Instead, it was called with " + newCalledOnString + " (" + newCalledOn.TES5Type.OriginalName + " : " + newCalledOn.TES5Type.NativeType.Name + ").");
            }
            return this.objectCallFactory.CreateObjectCall(newCalledOn, this.newFunctionName, this.objectCallArgumentsFactory.CreateArgumentList(functionArguments, codeScope, globalScope, multipleScriptsScope));
        }
    }
}