using Skyblivion.ESReader.Extensions;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using System;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetPlayerControlsDisabledFactory : IFunctionFactory//WTM:  Change:  Added
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5StaticReferenceFactory staticReferenceFactory;
        public GetPlayerControlsDisabledFactory(TES5ObjectCallFactory objectCallFactory, TES5StaticReferenceFactory staticReferenceFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.staticReferenceFactory = staticReferenceFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            //WTM:  Note:  Just like DisablePlayerControlsFactory, I want to emulate this:
            //https://cs.elderscrolls.com/index.php?title=DisablePlayerControls
            //Player cannot move, wait, activate anything, or access his journal interface. 
            string[] functionNames = new string[] { "IsMovementControlsEnabled", "IsMenuControlsEnabled"/*closest to "IsWaitControlsEnabled" I could find*/, "IsActivateControlsEnabled", "IsJournalControlsEnabled" };
            TES5ObjectCall[] objectCalls = functionNames.Select(f => objectCallFactory.CreateObjectCall(staticReferenceFactory.Game, f)).ToArray();
            ITES5ValueCodeChunk? accumulatedStatement = null;
            foreach (TES5ObjectCall objectCall in objectCalls)
            {
                if (accumulatedStatement == null)
                {
                    accumulatedStatement = objectCall;
                }
                else
                {
                    accumulatedStatement = TES5ExpressionFactory.CreateLogicalExpression(accumulatedStatement, TES5LogicalExpressionOperator.OPERATOR_AND, objectCall);
                }
            }
            if (accumulatedStatement == null) { throw new NullableException(nameof(accumulatedStatement)); }
            TES5ComparisonExpression negatedExpression = TES5ExpressionFactory.CreateComparisonExpression(accumulatedStatement, TES5ComparisonExpressionOperator.OPERATOR_NOT_EQUAL, new TES5Bool(true));
            return negatedExpression;
        }
    }
}
