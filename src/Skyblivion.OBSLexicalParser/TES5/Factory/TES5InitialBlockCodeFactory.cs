using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5InitialBlockCodeFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        public TES5InitialBlockCodeFactory(TES5ObjectCallFactory objectCallFactory)
        {
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
        public TES5CodeScope AddInitialCode(TES5GlobalScope globalScope, TES5EventCodeBlock eventCodeBlock)
        {
            switch (eventCodeBlock.BlockName)
            {
                case "OnUpdate":
                    {
                        if (globalScope.ScriptHeader.ScriptType.NativeType == TES5BasicType.T_QUEST)
                        {
                            TES5Branch branch = TES5BranchFactory.CreateSimpleBranch(TES5ExpressionFactory.CreateComparisonExpression(this.objectCallFactory.CreateObjectCall(TES5ReferenceFactory.CreateReferenceToSelf(globalScope), "IsRunning", new TES5ObjectCallArguments()), TES5ComparisonExpressionOperator.OPERATOR_EQUAL, new TES5Bool(false)), eventCodeBlock.CodeScope.LocalScope);
                            //Even though we"d like this script to not do anything at this time, it seems like sometimes condition races, so we"re putting it into a loop anyways but with early return bailout
                            branch.MainBranch.CodeScope.AddChunk(this.objectCallFactory.CreateRegisterForSingleUpdate(globalScope));
                            branch.MainBranch.CodeScope.AddChunk(new TES5Return());
                            eventCodeBlock.AddChunk(branch);
                            return eventCodeBlock.CodeScope;
                        }
                        /*else if (globalScope.ScriptHeader.BasicScriptType == TES5BasicType.T_OBJECTREFERENCE)
                        {
                            TES5LocalScope localScope = eventCodeBlock.CodeScope.LocalScope;
                            TES5Branch branch = TES5BranchFactory.CreateSimpleBranch(TES5ExpressionFactory.CreateComparisonExpression(this.objectCallFactory.CreateObjectCall(TES5ReferenceFactory.CreateReferenceToSelf(globalScope), "GetParentCell", multipleScriptsScope, new TES5ObjectCallArguments()), TES5ComparisonExpressionOperator.OPERATOR_EQUAL, this.objectCallFactory.CreateObjectCall(TES5ReferenceFactory.CreateReferenceToPlayer(), "GetParentCell", multipleScriptsScope, new TES5ObjectCallArguments())), localScope);
                            eventCodeBlock.AddChunk(branch);
                            return branch.MainBranch.CodeScope;
                        }*/
                        else
                        {
                            return eventCodeBlock.CodeScope;
                        }
                    }

                default:
                    {
                        return eventCodeBlock.CodeScope;
                    }
            }
        }
    }
}