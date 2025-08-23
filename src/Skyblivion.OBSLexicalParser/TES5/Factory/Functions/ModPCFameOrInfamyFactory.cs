using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class ModPCFameOrInfamyFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly string referenceName;
        public ModPCFameOrInfamyFactory(TES5ReferenceFactory referenceFactory, TES5ObjectCallFactory objectCallFactory, string referenceName)
        {
            this.referenceFactory = referenceFactory;
            this.objectCallFactory = objectCallFactory;
            this.referenceName = referenceName;
        }

        public ITES5CodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            var reference = this.referenceFactory.CreateReference(referenceName, globalScope, multipleScriptsScope, codeScope.LocalScope);
            var arguments = new TES5ObjectCallArguments() { new TES5Integer(((TES4Integer)function.Arguments[0]).IntValue) };
            return this.objectCallFactory.CreateObjectCall(reference, "Mod", arguments, comment: function.Comment);
        }
    }
}