using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetIsCreatureFactory : IFunctionFactory
    {
        private readonly GetIsPlayableRaceFactory getIsPlayableRaceFactory;
        public GetIsCreatureFactory(TES5ObjectCallFactory objectCallFactory)
        {
            //WTM:  Note:  GetIsCreature is not supported in Skyrim:  https://www.creationkit.com/index.php?title=GetIsCreature
            //I'm going to use GetIsPlayableRace.
            getIsPlayableRaceFactory = new GetIsPlayableRaceFactory(objectCallFactory);
        }

        public ITES5CodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            ITES5Value isPlayable = (ITES5Value)getIsPlayableRaceFactory.ConvertFunction(calledOn, function, codeScope, globalScope, multipleScriptsScope);
            TES5ComparisonExpression comparison = TES5ExpressionFactory.CreateFalseBooleanExpression(isPlayable);
            return comparison;
        }
    }
}
