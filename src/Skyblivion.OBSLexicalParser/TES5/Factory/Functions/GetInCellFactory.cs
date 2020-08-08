using Skyblivion.ESReader.TES4;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using System;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetInCellFactory : IFunctionFactory
    {
        private readonly ESMAnalyzer esmAnalyzer;
        private readonly TES5ObjectCallFactory objectCallFactory;
        public GetInCellFactory(TES5ObjectCallFactory objectCallFactory, ESMAnalyzer esmAnalyzer)
        {
            this.esmAnalyzer = esmAnalyzer;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            //WTM:  Note:
            //@INCONSISTENCE:  GetInCell checks if a cell name starts with the argument string:  https://cs.elderscrolls.com/index.php?title=GetInCell
            //The below will probably not always work.
            TES4FunctionArguments functionArguments = function.Arguments;
            ITES4StringValue apiToken = functionArguments[0];
            string argument = apiToken.StringValue;
            TES5ObjectCall getParentCell = this.objectCallFactory.CreateObjectCall(calledOn, "GetParentCell");
            TES5ObjectCall getParentCellName = this.objectCallFactory.CreateObjectCall(getParentCell, "GetName");
            TES4LoadedRecord cellRecord = this.esmAnalyzer.GetRecordByEDIDInTES4Collection(argument);
            string? cellNameWithSpaces = cellRecord.GetSubrecordTrimNullable("FULL");
            if (cellNameWithSpaces == null || cellNameWithSpaces.IndexOf("Dummy", StringComparison.OrdinalIgnoreCase) != -1) { cellNameWithSpaces = argument; }
            TES5String cellNameTES5String = new TES5String(cellNameWithSpaces);
            TES5ObjectCallArguments findArguments = new TES5ObjectCallArguments()
            {
                getParentCellName,
                cellNameTES5String
            };
            TES5ObjectCall substring = this.objectCallFactory.CreateObjectCall(TES5StaticReferenceFactory.StringUtil, "Find", findArguments);
            return TES5ExpressionFactory.CreateComparisonExpression(substring, TES5ComparisonExpressionOperator.OPERATOR_EQUAL, new TES5Integer(0));
        }
    }
}