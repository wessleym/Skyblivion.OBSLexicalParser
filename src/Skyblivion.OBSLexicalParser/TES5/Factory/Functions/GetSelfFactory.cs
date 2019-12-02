using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetSelfFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        public GetSelfFactory(TES5ReferenceFactory referenceFactory)
        {
            this.referenceFactory = referenceFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            //WTM:  Change:  I added these if statements.  Previously, this method generated implicit references only.
            if (calledOn.TES5Type == TES5BasicType.T_FORM)
            {
                return this.referenceFactory.CreateReadReference(calledOn.Name, globalScope, multipleScriptsScope, codeScope.LocalScope);
            }
            if (calledOn.TES5Type == TES5BasicType.T_ACTOR || calledOn.TES5Type.NativeType == TES5BasicType.T_OBJECTREFERENCE || calledOn.TES5Type.NativeType == TES5BasicType.T_ACTOR)
            {
                return this.referenceFactory.ExtractImplicitReference(globalScope, multipleScriptsScope, codeScope.LocalScope);
            }
            throw new ConversionException(nameof(GetSelfFactory) + ":  Unrecognized type:  " + calledOn.TES5Type.OriginalName);
        }
    }
}