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
        private TES5ReferenceFactory referenceFactory;
        private TES5ObjectCallFactory objectCallFactory;
        private TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public SetOwnershipFactory(TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory)
        {
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.referenceFactory = referenceFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk convertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments functionArguments = function.Arguments;
            TES5ObjectCallArguments args;
            string functionName;
            if (functionArguments.Any())
            {
                args = this.objectCallArgumentsFactory.createArgumentList(functionArguments, codeScope, globalScope, multipleScriptsScope);
                ITES5Type arg0Type = ESMAnalyzer._instance().getFormTypeByEDID(functionArguments[0].StringValue);
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
                args = new TES5ObjectCallArguments();
                args.Add(this.objectCallFactory.CreateGetActorBaseOfPlayer(multipleScriptsScope));
            }

            return this.objectCallFactory.CreateObjectCall(calledOn, functionName, multipleScriptsScope, args);
        }
    }
}