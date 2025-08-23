using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class ModDispositionFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        private readonly LogUnknownFunctionFactory logUnknownFunctionFactory;
        public ModDispositionFactory(TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, LogUnknownFunctionFactory logUnknownFunctionFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.logUnknownFunctionFactory = logUnknownFunctionFactory;
        }

        public ITES5CodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments functionArguments = function.Arguments;
            //Faction Reactions are Per-Faction not per-Actor, so we just simulate what would potentially happen in Skyrim
            var argument1 = functionArguments[1];
            Nullable<int> argument1NullableInt = argument1.Constant as Nullable<int>;
            if (argument1NullableInt != null && argument1NullableInt.Value == -100)
            {
                const string functionName = "StartCombat";
                functionArguments = functionArguments.RemoveInNew(1);
                return this.objectCallFactory.CreateObjectCall(calledOn, functionName, this.objectCallArgumentsFactory.CreateArgumentList(functionArguments, codeScope, globalScope, multipleScriptsScope));
            }
            else
            {
                if (TES5PlayerReference.EqualsPlayer(functionArguments[0].StringValue))
                {
                    TES4FunctionArguments argumentsAfterFirst = functionArguments.RemoveInNew(0);
                    var arguments = this.objectCallArgumentsFactory.CreateArgumentList(argumentsAfterFirst, codeScope, globalScope, multipleScriptsScope);
                    arguments.Insert(0, calledOn);
                    return this.objectCallFactory.CreateObjectCall(TES5StaticReferenceFactory.Create(TES5BasicType.T_TES4DISPOSITIONUTIL), "ModDisposition", arguments);
                }
                return logUnknownFunctionFactory.CreateLogCall(calledOn, function, globalScope);
            }
        }
    }
}