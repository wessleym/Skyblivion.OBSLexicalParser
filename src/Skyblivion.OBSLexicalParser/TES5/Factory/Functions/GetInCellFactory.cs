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
            //I tried adding GetName(), but in the case of TES4FGD03ViranusScript, it returned "Nonwyll Cavern" which fails to match "NonwyllCavern" from Oblivion's script.
            //So I'm now trying to add spaces between the pascal-case words.  If I could do something like GetName().Replace(" ", ""), and then compare strings case-insensitively,
            //that would be best.  But I don't know if that's possible in Papyrus and with this script converter.
            string cellNameWithSpaces = cellName.Select(c => c.ToString()).Aggregate((accumluatedString, currentCharacter) =>
                {
                    return accumluatedString +
                    ((char.IsUpper(currentCharacter[0]) || char.IsNumber(currentCharacter[0])) && !(char.IsUpper(accumluatedString[accumluatedString.Length - 1]) || char.IsNumber(accumluatedString[accumluatedString.Length - 1])) ? " " : "") +
                    currentCharacter;
                });
            cellNameWithSpaces = cellNameWithSpaces.Replace("ofthe ", " of the ");
            cellNameWithSpaces = cellNameWithSpaces.Replace("I C ", "IC ");
            cellNameWithSpaces = cellNameWithSpaces.Replace("M Q", "MQ ");
            cellNameWithSpaces = cellNameWithSpaces.Replace("Chapelof ", "Chapel of ");
            TES5String cellNameTES5String = new TES5String(cellNameWithSpaces);
            return TES5ExpressionFactory.createArithmeticExpression(getParentCellName, TES5ArithmeticExpressionOperator.OPERATOR_EQUAL, cellNameTES5String);
        }
    }
}