using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using System;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class ModDispositionFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public ModDispositionFactory(TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments functionArguments = function.Arguments;
            //Faction Reactions are Per-Faction not per-Actor, so we just simulate what would potentially happen in Skyrim
            var argument1 = functionArguments[1];
            Nullable<int> argument1NullableInt = argument1.Data as Nullable<int>;
            if (argument1NullableInt != null && argument1NullableInt.Value==-100)
            {
                const string functionName = "StartCombat";
                functionArguments.RemoveAt(1);
                return this.objectCallFactory.CreateObjectCall(calledOn, functionName, this.objectCallArgumentsFactory.CreateArgumentList(functionArguments, codeScope, globalScope, multipleScriptsScope));
            }
            else
            {
                return new TES5Filler();
            }
        }
    }
}