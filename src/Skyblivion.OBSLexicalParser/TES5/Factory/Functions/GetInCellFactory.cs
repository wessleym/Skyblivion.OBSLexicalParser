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
using Skyblivion.OBSLexicalParser.TES5.Service;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetInCellFactory : IFunctionFactory
    {
        private TES5ReferenceFactory referenceFactory;
        private TES5VariableAssignationFactory assignationFactory;
        private ESMAnalyzer analyzer;
        private TES5ObjectPropertyFactory objectPropertyFactory;
        private TES5TypeInferencer typeInferencer;
        private MetadataLogService metadataLogService;
        private TES5ValueFactory valueFactory;
        private TES5ObjectCallFactory objectCallFactory;
        private TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public GetInCellFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, TES5VariableAssignationFactory assignationFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer analyzer,TES5TypeInferencer typeInferencer, MetadataLogService metadataLogService)
        {
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.valueFactory = valueFactory;
            this.referenceFactory = referenceFactory;
            this.analyzer = analyzer;
            this.assignationFactory = assignationFactory;
            this.objectPropertyFactory = objectPropertyFactory;
            this.typeInferencer = typeInferencer;
            this.metadataLogService = metadataLogService;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk convertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments functionArguments = function.Arguments;
            ITES4StringValue apiToken = functionArguments[0];
            string cellName = apiToken.StringValue;
            TES5ObjectCall getParentCell = this.objectCallFactory.CreateObjectCall(calledOn, "GetParentCell", multipleScriptsScope);
            TES5ObjectCall getParentCellName = this.objectCallFactory.CreateObjectCall(getParentCell, "GetName", multipleScriptsScope);
            /*int length = cellName.Length;
            TES5ObjectCallArguments substringArguments = new TES5ObjectCallArguments();
            substringArguments.Add(getParentCellName);
            substringArguments.Add(new TES5Integer(0));
            substringArguments.Add(new TES5Integer(length));
            TES5ObjectCall substring = this.objectCallFactory.CreateObjectCall(TES5StaticReference.StringUtil, "Substring", multipleScriptsScope, substringArguments);
            TES5String cellNameTES5String = new TES5String(cellName);
            return TES5ExpressionFactory.createArithmeticExpression(substring, TES5ArithmeticExpressionOperator.OPERATOR_EQUAL, cellNameTES5String);
            */
            //WTM:  Change:  The above method doesn't work.  It originally used GetParentCell(), which seems to have a string value of just "[Cell" (no closing right bracket).
            //Skyrim's GetName() gets a name with spaces.  To get the same name in Oblivion, I need to take the name that is present in Oblivion's scripts and look it up in the ESMAnalyzer.
            //This required adding "FULL" to the records that are collected from "CELL."  If the below method becomes unused, "FULL" records will no longer be necessary for "CELL."
            TES4LoadedRecord cellRecord = ESMAnalyzer._instance().FindInTES4Collection(cellName, false);
            string cellNameWithSpaces = cellRecord.getSubrecordTrim("FULL");
            if (cellNameWithSpaces == null) { cellNameWithSpaces = cellName; }
            TES5String cellNameTES5String = new TES5String(cellNameWithSpaces);
            return TES5ExpressionFactory.createArithmeticExpression(getParentCellName, TES5ArithmeticExpressionOperator.OPERATOR_EQUAL, cellNameTES5String);
        }
    }
}