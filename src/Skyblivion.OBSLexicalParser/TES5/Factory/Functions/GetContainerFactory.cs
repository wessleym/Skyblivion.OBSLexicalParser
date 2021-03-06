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
        public GetContainerFactory() { }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            //TODO: This function is nonexistent in Papyrus and most likely should be deleted
            TES5SignatureParameter? containerParameter = codeScope.TryGetFunctionParameterByMeaning(TES5LocalVariableParameterMeaning.CONTAINER);
            if (containerParameter == null)
            {
                throw new ConversionException("GetContainer:  Cannot convert to Skyrim in other contexts than onEquip/onUnequip", expected: true);
            }
            return TES5ReferenceFactory.CreateReferenceToVariableOrProperty(containerParameter);
        }
    }
}