using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetActionRefFactory : IFunctionFactory
    {
        public GetActionRefFactory() { }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5SignatureParameter? activatorParameter = codeScope.TryGetFunctionParameterByMeaning(TES5LocalVariableParameterMeaning.ACTIVATOR);
            if (activatorParameter == null)
            {
                throw new ConversionException("getActionRef in non-activator scope found. Cannot convert that one.", expected: true);
            }
            return TES5ReferenceFactory.CreateReferenceToVariableOrProperty(activatorParameter);
        }
    }
}