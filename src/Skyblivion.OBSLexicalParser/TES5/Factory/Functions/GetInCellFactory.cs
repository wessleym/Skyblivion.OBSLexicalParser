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

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetInCellFactory : IFunctionFactory
    {
        private readonly ESMAnalyzer analyzer;
        private readonly TES5ObjectCallFactory objectCallFactory;
        public GetInCellFactory(TES5ObjectCallFactory objectCallFactory, ESMAnalyzer analyzer)
        {
            this.analyzer = analyzer;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            //GetInCell checks if a cell name starts with the argument string:  https://cs.elderscrolls.com/index.php?title=GetInCell
            //The below will probably not always work.
            TES4FunctionArguments functionArguments = function.Arguments;
            ITES4StringValue apiToken = functionArguments[0];
            string cellName = apiToken.StringValue;
            TES5ObjectCall getParentCell = this.objectCallFactory.CreateObjectCall(calledOn, "GetParentCell");
            TES5ObjectCall getParentCellName = this.objectCallFactory.CreateObjectCall(getParentCell, "GetName");
            int length = cellName.Length;
            TES5ObjectCallArguments substringArguments = new TES5ObjectCallArguments()
            {
                getParentCellName,
                new TES5Integer(0),
                new TES5Integer(length)
            };
            TES5ObjectCall substring = this.objectCallFactory.CreateObjectCall(TES5StaticReference.StringUtil, "Substring", substringArguments);
            TES4LoadedRecord cellRecord = this.analyzer.FindInTES4Collection(cellName);
            string? cellNameWithSpaces = cellRecord.GetSubrecordTrim("FULL");
            if (cellNameWithSpaces == null) { cellNameWithSpaces = cellName; }
            TES5String cellNameTES5String = new TES5String(cellNameWithSpaces);
            return TES5ExpressionFactory.CreateComparisonExpression(substring, TES5ComparisonExpressionOperator.OPERATOR_EQUAL, cellNameTES5String);
        }
    }
}