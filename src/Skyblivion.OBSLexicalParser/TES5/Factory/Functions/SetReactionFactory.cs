using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SetFactionReactionFactory : SetOrModFactionReactionFactory, IFunctionFactory
    {
        public SetFactionReactionFactory(TES5ReferenceFactory referenceFactory, TES5ObjectCallFactory objectCallFactory)
            : base(referenceFactory, objectCallFactory)
        { }

        public ITES5CodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            int oldReaction = int.Parse(function.Arguments[2].StringValue);
            return ConvertFunction(function, codeScope, globalScope, multipleScriptsScope, oldReaction > 0, oldReaction >= 0);
        }
    }
}
