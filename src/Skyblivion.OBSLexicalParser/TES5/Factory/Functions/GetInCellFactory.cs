using Skyblivion.ESReader.TES4;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Other;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetInCellFactory : IFunctionFactory
    {
        private readonly ESMAnalyzer esmAnalyzer;
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly CellToLocationFinder cellToLocationFinder;
        public GetInCellFactory(TES5ObjectCallFactory objectCallFactory, ESMAnalyzer esmAnalyzer)
        {
            this.esmAnalyzer = esmAnalyzer;
            this.objectCallFactory = objectCallFactory;
            cellToLocationFinder = new CellToLocationFinder();
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments functionArguments = function.Arguments;
            ITES4StringValue apiToken = functionArguments[0];
            string argument = apiToken.StringValue;
            string locationPropertyNameWithoutSuffix = argument + "Location";
            string locationPropertyNameWithSuffix = TES5Property.AddPropertyNameSuffix(locationPropertyNameWithoutSuffix);
            TES5Property? locationProperty = globalScope.TryGetPropertyByName(locationPropertyNameWithSuffix);
            if (locationProperty == null)
            {
                int tes5FormID;
                if (cellToLocationFinder.TryGetLocationFormID(argument, out tes5FormID))
                {
                    locationProperty = TES5PropertyFactory.ConstructWithTES5FormID(locationPropertyNameWithoutSuffix, TES5BasicType.T_LOCATION, null, tes5FormID);
                    globalScope.AddProperty(locationProperty);
                }
                else
                {
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
            TES5Reference locationReference = TES5ReferenceFactory.CreateReferenceToVariableOrProperty(locationProperty);
            return this.objectCallFactory.CreateObjectCall(calledOn, "IsInLocation", new TES5ObjectCallArguments() { locationReference });
        }
    }
}