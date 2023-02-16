using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class ForceWeatherFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        public ForceWeatherFactory(TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory)
        {
            this.referenceFactory = referenceFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4FunctionArguments functionArguments = function.Arguments;
            ITES5Referencer newCalledOn = this.referenceFactory.CreateReadReference(functionArguments[0].StringValue, globalScope, multipleScriptsScope, localScope);
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments();
            TES5Bool force;
            if (functionArguments.Count == 1)
            {
                force = new TES5Bool(false);
            }
            else
            {
                force = new TES5Bool(((TES4Integer)functionArguments[1]).IntValue == 1);
            }
            arguments.Add(force);
            return this.objectCallFactory.CreateObjectCall(newCalledOn, "ForceActive", arguments, comment: function.Comment);
        }
    }
}