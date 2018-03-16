using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Converter;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5InitialBlockCodeFactory
    {
        private TES5BranchFactory branchFactory;
        private TES5ExpressionFactory expressionFactory;
        private TES5ReferenceFactory referenceFactory;
        private TES5ObjectCallFactory objectCallFactory;
        public TES5InitialBlockCodeFactory(TES5BranchFactory branchFactory, TES5ExpressionFactory expressionFactory, TES5ReferenceFactory referenceFactory, TES5ObjectCallFactory objectCallFactory)
        {
            this.branchFactory = branchFactory;
            this.expressionFactory = expressionFactory;
            this.referenceFactory = referenceFactory;
            this.objectCallFactory = objectCallFactory;
        }

        /*
        * Add initial code to the blocks and return the scope in which conversion should occur
         * Sometimes, we want to add a bit of code before the converted code, or want to encapsulate whole converted code
         * with a branch or so - this is a place to do it.
         * 
         * 
         * 
         *  Scope in which we want for conversion to happen
        */
        public TES5CodeScope addInitialCode(TES5MultipleScriptsScope multipleScriptsScope, TES5GlobalScope globalScope, TES5EventCodeBlock eventCodeBlock)
        {
            switch (eventCodeBlock.getBlockType())
            {
                case "OnUpdate":
                    {
                        if (globalScope.getScriptHeader().getBasicScriptType() == TES5BasicType.T_QUEST)
                        {
                            TES5Branch branch = this.branchFactory.createSimpleBranch(this.expressionFactory.createArithmeticExpression(this.objectCallFactory.createObjectCall(this.referenceFactory.createReferenceToSelf(globalScope), "IsRunning", multipleScriptsScope, new TES5ObjectCallArguments()), TES5ArithmeticExpressionOperator.OPERATOR_EQUAL, new TES5Bool(false)), eventCodeBlock.getCodeScope().getLocalScope());
                            //Even though we"d like this script to not do anything at this time, it seems like sometimes condition races, so we"re putting it into a loop anyways but with early return bailout
                            TES5ObjectCallArguments args = new TES5ObjectCallArguments();
                            args.add(new TES5Float(TES5AdditionalBlockChangesPass.ON_UPDATE_TICK));
                            branch.getMainBranch().getCodeScope().add(this.objectCallFactory.createObjectCall(this.referenceFactory.createReferenceToSelf(globalScope), "RegisterForSingleUpdate", multipleScriptsScope, args));
                            branch.getMainBranch().getCodeScope().add(new TES5Return());
                            eventCodeBlock.addChunk(branch);
                            return eventCodeBlock.getCodeScope();
                        }

                        else if (globalScope.getScriptHeader().getBasicScriptType() == TES5BasicType.T_OBJECTREFERENCE)
                        {
                            TES5Branch branch = this.branchFactory.createSimpleBranch(this.expressionFactory.createArithmeticExpression(this.objectCallFactory.createObjectCall(this.referenceFactory.createReferenceToSelf(globalScope), "GetParentCell", multipleScriptsScope, new TES5ObjectCallArguments()), TES5ArithmeticExpressionOperator.OPERATOR_EQUAL, this.objectCallFactory.createObjectCall(this.referenceFactory.createReferenceToPlayer(), "GetParentCell", multipleScriptsScope, new TES5ObjectCallArguments())), eventCodeBlock.getCodeScope().getLocalScope());
                            eventCodeBlock.addChunk(branch);
                            return branch.getMainBranch().getCodeScope();
                        }
                        else
                        {
                            return eventCodeBlock.getCodeScope();
                        }
                    }

                default:
                    {
                        return eventCodeBlock.getCodeScope();
                    }
            }
        }
    }
}