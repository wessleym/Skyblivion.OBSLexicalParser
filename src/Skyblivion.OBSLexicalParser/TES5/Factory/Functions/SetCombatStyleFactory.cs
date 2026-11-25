using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SetCombatStyleFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        private readonly TES5ReferenceFactory referenceFactory;
        public SetCombatStyleFactory(TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.referenceFactory = referenceFactory;
        }

        public ITES5CodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments functionArguments = function.Arguments;
            TES5ObjectCall getActorBase = this.objectCallFactory.CreateGetActorBase(calledOn);
            TES5ObjectCallArguments newArguments = this.objectCallArgumentsFactory.CreateArgumentList(functionArguments, codeScope, globalScope, multipleScriptsScope);
            TES5ObjectCall GetSetCombatStyle() { return this.objectCallFactory.CreateObjectCall(getActorBase, "SetCombatStyle", newArguments, comment: function.Comment); }
            const string originalCombatStyleName = "OriginalCombatStyle";
            if (newArguments.Count > 0)
            {
                TES5Property originalCombatStyle = new TES5Property(originalCombatStyleName, TES5BasicType.T_COMBATSTYLE, null, null, null);
                globalScope.AddPropertyIfNotExists(originalCombatStyle);
                TES5CodeChunkCollection codeChunks = new TES5CodeChunkCollection
                {
                    TES5VariableAssignationFactory.CreateAssignation(TES5ReferenceFactory.CreateReferenceToVariableOrProperty(originalCombatStyle), this.objectCallFactory.CreateObjectCall(getActorBase, "GetCombatStyle")),
                    GetSetCombatStyle()
                };
                return codeChunks;
            }
            {
                ITES5Referencer originalCombatStyle = referenceFactory.CreateReadReference(originalCombatStyleName, globalScope, multipleScriptsScope, codeScope.LocalScope);
                TES5IfElseChain ifElseChain = TES5IfElseFactory.CreateIfElseChain(TES5ExpressionFactory.CreateComparisonExpression(originalCombatStyle, TES5ComparisonExpressionOperator.OPERATOR_NOT_EQUAL, new TES5None()), codeScope.LocalScope);
                newArguments.Add(originalCombatStyle);
                ifElseChain.IfClause.CodeScope.AddChunk(GetSetCombatStyle());
                return ifElseChain;
            }
        }
    }
}
