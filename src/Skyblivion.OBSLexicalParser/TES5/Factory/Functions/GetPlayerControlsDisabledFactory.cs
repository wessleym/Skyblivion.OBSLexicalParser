﻿using Skyblivion.ESReader.Extensions;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetPlayerControlsDisabledFactory : IFunctionFactory//WTM:  Change:  Added
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        public GetPlayerControlsDisabledFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            if (function.Comment != null)
            {
                throw new ConversionException(function.FunctionCall.FunctionName + "'s comment could not be retained.");
            }
            //WTM:  Note:  Just like DisablePlayerControlsFactory, I want to emulate this:
            //https://cs.elderscrolls.com/index.php?title=DisablePlayerControls
            //Player cannot move, wait, activate anything, or access his journal interface. 
            string[] functionNames = new string[] { "IsMovementControlsEnabled", "IsMenuControlsEnabled"/*closest to "IsWaitControlsEnabled" I could find*/, "IsActivateControlsEnabled", "IsJournalControlsEnabled" };
            TES5ObjectCall[] objectCalls = functionNames.Select(f => objectCallFactory.CreateObjectCall(TES5StaticReferenceFactory.Game, f)).ToArray();
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
