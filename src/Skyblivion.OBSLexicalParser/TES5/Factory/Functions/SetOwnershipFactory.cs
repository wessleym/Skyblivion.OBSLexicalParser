using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SetOwnershipFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        private readonly ESMAnalyzer analyzer;
        public SetOwnershipFactory(TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, ESMAnalyzer analyzer)
        {
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.referenceFactory = referenceFactory;
            this.objectCallFactory = objectCallFactory;
            this.analyzer = analyzer;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments functionArguments = function.Arguments;
            TES5ObjectCallArguments args;
            string functionName;
            if (functionArguments.Any())
            {
                args = this.objectCallArgumentsFactory.CreateArgumentList(functionArguments, codeScope, globalScope, multipleScriptsScope);
                ITES5Type arg0Type = analyzer.GetFormTypeByEDID(functionArguments[0].StringValue);
                if (arg0Type == TES5BasicType.T_ACTOR)
                {
                    functionName = "SetActorOwner";
                }
                else if (arg0Type == TES5BasicType.T_FACTION)
                {
                    functionName = "SetFactionOwner";
                }
                else
                {
                    throw new ConversionException("Unknown setOwnership() param");
                }
            }
            else
            {
                functionName = "SetActorOwner";
                args = new TES5ObjectCallArguments() { this.objectCallFactory.CreateGetActorBaseOfPlayer(globalScope, multipleScriptsScope) };
            }

            return this.objectCallFactory.CreateObjectCall(calledOn, functionName, multipleScriptsScope, args);
        }
    }
}