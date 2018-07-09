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
    class GetContainerFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        public GetContainerFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            //TODO: This function is nonexistent in Papyrus and most likely should be deleted
            ITES5VariableOrProperty containerVariable = codeScope.GetVariableWithMeaning(TES5LocalVariableParameterMeaning.CONTAINER);
            if (containerVariable == null)
            {
                throw new ConversionException("GetContainer:  Cannot convert to Skyrim in other contexts than onEquip/onUnequip", expected: true);
            }
            return TES5ReferenceFactory.CreateReferenceToVariable(containerVariable);
        }
    }
}