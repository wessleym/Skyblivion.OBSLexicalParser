using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class IsSpellTargetFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        public IsSpellTargetFactory(TES5ReferenceFactory referenceFactory, TES5ObjectCallFactory objectCallFactory)
        {
            this.referenceFactory = referenceFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            //@INCONSISTENCE - Will only check for scripted effects
            //In oblivion, this is checking for a spell which targeted a given actor
            //In Skyrim you can check for effects only.
            TES5ObjectCallArguments newArgs = new TES5ObjectCallArguments()
            {
                this.referenceFactory.CreateReference("EffectSEFF", TES5BasicType.T_MAGICEFFECT, globalScope, multipleScriptsScope, localScope)
            };
            return this.objectCallFactory.CreateObjectCall(calledOn, "HasMagicEffect", newArgs);
        }
    }
}