using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Context;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class ActivateFactory : IFunctionFactory
    {
        private readonly TES5ValueFactory valueFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        public ActivateFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory)
        {
            this.valueFactory = valueFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments functionArguments = function.Arguments;
            if (!functionArguments.Any())
            {
                TES5ObjectCallArguments constantArgumentForNoFunctionArguments = new TES5ObjectCallArguments();
                TES5SignatureParameter? parameterByMeaning = codeScope.TryGetFunctionParameterByMeaning(TES5LocalVariableParameterMeaning.ACTIVATOR);
                if (parameterByMeaning != null)
                {
                    constantArgumentForNoFunctionArguments.Add(TES5ReferenceFactory.CreateReferenceToVariableOrProperty(parameterByMeaning));
                }
                else
                {
                    constantArgumentForNoFunctionArguments.Add(TES5ReferenceFactory.CreateReferenceToPlayer(globalScope));
                }

                constantArgumentForNoFunctionArguments.Add(new TES5Bool(true)); //Since default in oblivion is "skip the OnActivateBlock", this defaults to "abDefaultProcessingOnly = true" in Skyrim
                return this.objectCallFactory.CreateObjectCall(calledOn, function, constantArgumentForNoFunctionArguments);
            }

            TES5ObjectCallArguments constantArgument = new TES5ObjectCallArguments() { this.valueFactory.CreateValue(functionArguments[0], codeScope, globalScope, multipleScriptsScope) };
            ITES4ValueString? blockOnActivate = functionArguments.GetOrNull(1);
            bool argument1Bool;
            if (blockOnActivate != null)
            {
                bool blockOnActivateVal = (int)blockOnActivate.Constant == 1;
                argument1Bool = !blockOnActivateVal;
            }
            else
            {
                argument1Bool = true;
            }
            constantArgument.Add(new TES5Bool(argument1Bool));

            return this.objectCallFactory.CreateObjectCall(calledOn, function, constantArgument);
        }
    }
}