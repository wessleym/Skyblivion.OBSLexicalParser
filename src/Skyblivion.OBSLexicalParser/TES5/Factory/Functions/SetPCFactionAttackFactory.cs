using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SetPCFactionAttackFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ReferenceFactory referenceFactory;
        public SetPCFactionAttackFactory(TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.referenceFactory = referenceFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4FunctionArguments functionArguments = function.Arguments;
            int arg;
            switch (((TES4Integer)functionArguments[1]).IntValue)
            {
                case 0:
                {
                    arg = 0;
                    break;
                }

                case 1:
                {
                    arg = 1000;
                    break;
                }

                default:
                {
                    throw new ConversionException("SetPCFactionMurder/SetPCFactionAttack argument unknown");
                }
            }

            TES5ObjectCallArguments constantArgument = new TES5ObjectCallArguments() { new TES5Integer(arg) };
            ITES5Referencer faction = this.referenceFactory.CreateReadReference(functionArguments[0].StringValue, globalScope, multipleScriptsScope, localScope);
            TES5ObjectCall newFunction = this.objectCallFactory.CreateObjectCall(faction, "SetCrimeGoldViolent", constantArgument, comment: function.Comment);
            return newFunction;
        }
    }
}