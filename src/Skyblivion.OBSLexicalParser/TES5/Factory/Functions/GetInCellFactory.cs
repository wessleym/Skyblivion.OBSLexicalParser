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
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetInCellFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly CellToLocationFinder cellToLocationFinder;
        private readonly ESMAnalyzer esmAnalyzer;
        public GetInCellFactory(TES5ObjectCallFactory objectCallFactory, CellToLocationFinder cellToLocationFinder, ESMAnalyzer esmAnalyzer)
        {
            this.objectCallFactory = objectCallFactory;
            this.cellToLocationFinder = cellToLocationFinder;
            this.esmAnalyzer = esmAnalyzer;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments functionArguments = function.Arguments;
            ITES4StringValue apiToken = functionArguments[0];
            string cellEditorID = apiToken.StringValue;
            string locationPropertyNameWithoutSuffix = cellEditorID + "Location";
            string locationPropertyNameWithSuffix = TES5Property.AddPropertyNameSuffix(locationPropertyNameWithoutSuffix);
            TES5Property? locationProperty = globalScope.TryGetPropertyByName(locationPropertyNameWithSuffix);
            if (locationProperty == null)
            {
                int tes5LocationFormID;
                if (cellToLocationFinder.TryGetLocationFormID(cellEditorID, out tes5LocationFormID))
                {
                    locationProperty = TES5PropertyFactory.ConstructWithTES5FormID(locationPropertyNameWithoutSuffix, TES5BasicType.T_LOCATION, null, tes5LocationFormID);
                    globalScope.AddProperty(locationProperty);
                }
                else
                {
                    TES5ObjectCall getParentCell = this.objectCallFactory.CreateObjectCall(calledOn, "GetParentCell");
                    TES5ObjectCall getParentCellName = this.objectCallFactory.CreateObjectCall(getParentCell, "GetName");
                    TES4LoadedRecord cellRecord = this.esmAnalyzer.GetRecordByEDIDInTES4Collection(cellEditorID);
                    string? cellNameWithSpaces = cellRecord.GetSubrecordTrimNullable("FULL");
                    if (cellNameWithSpaces == null || cellNameWithSpaces.IndexOf("Dummy", StringComparison.OrdinalIgnoreCase) != -1) { cellNameWithSpaces = cellEditorID; }
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