using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
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
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public SetOwnershipFactory(TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory)
        {
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments functionArguments = function.Arguments;
            TES5ObjectCallArguments args;
            string functionName;
            if (functionArguments.Any())
            {
                args = this.objectCallArgumentsFactory.CreateArgumentList(functionArguments, codeScope, globalScope, multipleScriptsScope);
                var arg0Type = args[0].TES5Type;
                if (TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(arg0Type, TES5BasicType.T_ACTORBASE))
                {
                    functionName = "SetActorOwner";
                }
                else if (TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(arg0Type, TES5BasicType.T_ACTOR))
                {
                    args[0] = objectCallFactory.CreateGetActorBase((ITES5Referencer)args[0]);
                    functionName = "SetActorOwner";
                }
                else if (TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(arg0Type, TES5BasicType.T_FACTION))
                {
                    functionName = "SetFactionOwner";
                }
                else
                {
                    throw new ConversionException(function.FunctionCall.FunctionName + " should be called with either an ActorBase or a Faction.  Instead, it was called with " + arg0Type.Value + " (" + arg0Type.OriginalName + " : " + arg0Type.NativeType.Name + ").");
                }
            }
            else
            {
                functionName = "SetActorOwner";
                args = new TES5ObjectCallArguments() { this.objectCallFactory.CreateGetActorBaseOfPlayer(globalScope) };
            }

            return this.objectCallFactory.CreateObjectCall(calledOn, functionName, args, comment: function.Comment);
        }
    }
}