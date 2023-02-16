using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using System;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SetPCInfamyFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly TES5ValueFactory valueFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        public SetPCInfamyFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory)
        {
            this.valueFactory = valueFactory;
            this.referenceFactory = referenceFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4FunctionArguments functionArguments = function.Arguments;
            TES5ObjectCallArguments fameArguments = new TES5ObjectCallArguments();
            ITES4ValueString argument0 = functionArguments[0];
            Nullable<int> argument0Int = argument0.Constant as Nullable<int>;
            ITES5Value newArgument;
            if (argument0Int != null)
            {
                newArgument = new TES5Integer(argument0Int.Value);
            }
            else
            {
                newArgument= this.valueFactory.CreateValue(argument0, codeScope, globalScope, multipleScriptsScope);
            }
            fameArguments.Add(newArgument);
            TES5ObjectCall newFunction = this.objectCallFactory.CreateObjectCall(this.referenceFactory.CreateReference("Infamy", globalScope, multipleScriptsScope, localScope), "SetValue", fameArguments, comment: function.Comment);
            return newFunction;
        }
    }
}