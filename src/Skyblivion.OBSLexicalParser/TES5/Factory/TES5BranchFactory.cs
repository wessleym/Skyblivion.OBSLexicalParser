using Skyblivion.OBSLexicalParser.TES4.AST.Code.Branch;
using Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Ormin.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5BranchFactory
    {
        private TES5LocalScopeFactory localScopeFactory;
        private TES5CodeScopeFactory codeScopeFactory;
        private TES5ChainedCodeChunkFactory codeChunkFactory;
        private TES5ValueFactory valueFactory;
        public TES5BranchFactory(TES5LocalScopeFactory localScopeFactory, TES5CodeScopeFactory codeScopeFactory, TES5ValueFactory valueFactory)
        {
            this.localScopeFactory = localScopeFactory;
            this.codeScopeFactory = codeScopeFactory;
            this.valueFactory = valueFactory;
        }

        //UGLY but w/e PLEASE FIX THAT PLEASEE :((
        public void setCodeChunkFactory(TES5ChainedCodeChunkFactory chainedCodeChunkFactory)
        {
            this.codeChunkFactory = chainedCodeChunkFactory;
        }

        public TES5CodeChunkCollection createCodeChunk(TES4Branch chunk, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5SubBranch mainBranch = this.convertSubBranch(chunk.getMainBranch(), codeScope, globalScope, multipleScriptsScope);
            TES4SubBranchList branchList = chunk.getElseifBranches();
            TES5SubBranchList convertedElseIfBranches = null;
            if (branchList != null)
            {
                convertedElseIfBranches = new TES5SubBranchList();
                foreach (TES4SubBranch subBranch in branchList.getSubBranches())
                {
                    convertedElseIfBranches.add(this.convertSubBranch(subBranch, codeScope, globalScope, multipleScriptsScope));
                }
            }

            TES4ElseSubBranch elseBranch = chunk.getElseBranch();
            TES5ElseSubBranch convertedElseBranch = null;
            if (elseBranch != null)
            {
                convertedElseBranch = this.convertElseBranch(elseBranch, codeScope, globalScope, multipleScriptsScope);
            }

            TES5CodeChunkCollection collection = new TES5CodeChunkCollection();
            collection.add(new TES5Branch(mainBranch, convertedElseIfBranches, convertedElseBranch));
            return collection;
        }

        public TES5Branch createSimpleBranch(ITES5Expression expression, TES5LocalScope parentScope)
        {
            return new TES5Branch(new TES5SubBranch(expression, this.codeScopeFactory.createCodeScope(this.localScopeFactory.createRecursiveScope(parentScope))));
        }

        private TES5ElseSubBranch convertElseBranch(TES4ElseSubBranch branch, TES5CodeScope outerCodeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope outerLocalScope = outerCodeScope.getLocalScope();
            TES5LocalScope newScope = this.localScopeFactory.createRecursiveScope(outerLocalScope);
            TES5CodeScope newCodeScope = this.codeScopeFactory.createCodeScope(newScope);
            TES4CodeChunks branchChunks = branch.getCodeChunks();
            if (branchChunks != null)
            {
                foreach (ITES4CodeChunk codeChunk in branchChunks.getCodeChunks())
                {
                    TES5CodeChunkCollection codeChunks = this.codeChunkFactory.createCodeChunk(codeChunk, newCodeScope, globalScope, multipleScriptsScope);
                    if (codeChunks != null)
                    {
                        foreach (ITES5CodeChunk newCodeChunk in codeChunks)
                        {
                            newCodeScope.add(newCodeChunk);
                        }
                    }
                }
            }

            return new TES5ElseSubBranch(newCodeScope);
        }

        private TES5SubBranch convertSubBranch(TES4SubBranch branch, TES5CodeScope outerCodeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope outerLocalScope = outerCodeScope.getLocalScope();
            ITES5Value expression = this.valueFactory.createValue(branch.getExpression(), outerCodeScope, globalScope, multipleScriptsScope);
            TES5LocalScope newScope = this.localScopeFactory.createRecursiveScope(outerLocalScope);
            TES5CodeScope newCodeScope = this.codeScopeFactory.createCodeScope(newScope);
            TES4CodeChunks branchChunks = branch.getCodeChunks();
            if (branchChunks != null)
            {
                foreach (ITES4CodeChunk codeChunk in branchChunks.getCodeChunks())
                {
                    TES5CodeChunkCollection codeChunks = this.codeChunkFactory.createCodeChunk(codeChunk, newCodeScope, globalScope, multipleScriptsScope);
                    if (codeChunks != null)
                    {
                        foreach (ITES5CodeChunk newCodeChunk in codeChunks)
                        {
                            newCodeScope.add(newCodeChunk);
                        }
                    }
                }
            }

            return new TES5SubBranch(expression, newCodeScope);
        }
    }
}