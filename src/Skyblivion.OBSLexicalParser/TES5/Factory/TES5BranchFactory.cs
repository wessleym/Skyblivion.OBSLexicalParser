using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.Code.Branch;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    static class TES5BranchFactory
    {
        public static TES5CodeChunkCollection CreateCodeChunk(TES4Branch chunk, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES5ChainedCodeChunkFactory codeChunkFactory, TES5ValueFactory valueFactory)
        {
            TES5SubBranch mainBranch = ConvertSubBranch(chunk.getMainBranch(), codeScope, globalScope, multipleScriptsScope, codeChunkFactory, valueFactory);
            TES4SubBranchList branchList = chunk.getElseifBranches();
            TES5SubBranchList convertedElseIfBranches = null;
            if (branchList != null)
            {
                convertedElseIfBranches = new TES5SubBranchList();
                foreach (TES4SubBranch subBranch in branchList.getSubBranches())
                {
                    convertedElseIfBranches.add(ConvertSubBranch(subBranch, codeScope, globalScope, multipleScriptsScope, codeChunkFactory, valueFactory));
                }
            }

            TES4ElseSubBranch elseBranch = chunk.getElseBranch();
            TES5ElseSubBranch convertedElseBranch = null;
            if (elseBranch != null)
            {
                convertedElseBranch = ConvertElseBranch(elseBranch, codeScope, globalScope, multipleScriptsScope, codeChunkFactory);
            }

            TES5CodeChunkCollection collection = new TES5CodeChunkCollection();
            collection.add(new TES5Branch(mainBranch, convertedElseIfBranches, convertedElseBranch));
            return collection;
        }

        public static TES5Branch CreateSimpleBranch(ITES5Expression expression, TES5LocalScope parentScope)
        {
            return new TES5Branch(new TES5SubBranch(expression, TES5CodeScopeFactory.createCodeScope(TES5LocalScopeFactory.createRecursiveScope(parentScope))));
        }

        private static TES5ElseSubBranch ConvertElseBranch(TES4ElseSubBranch branch, TES5CodeScope outerCodeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES5ChainedCodeChunkFactory codeChunkFactory)
        {
            TES5LocalScope outerLocalScope = outerCodeScope.LocalScope;
            TES5LocalScope newScope = TES5LocalScopeFactory.createRecursiveScope(outerLocalScope);
            TES5CodeScope newCodeScope = TES5CodeScopeFactory.createCodeScope(newScope);
            TES4CodeChunks branchChunks = branch.CodeChunks;
            if (branchChunks != null)
            {
                foreach (ITES4CodeChunk codeChunk in branchChunks.CodeChunks)
                {
                    TES5CodeChunkCollection codeChunks = codeChunkFactory.createCodeChunk(codeChunk, newCodeScope, globalScope, multipleScriptsScope);
                    if (codeChunks != null)
                    {
                        foreach (ITES5CodeChunk newCodeChunk in codeChunks)
                        {
                            newCodeScope.Add(newCodeChunk);
                        }
                    }
                }
            }

            return new TES5ElseSubBranch(newCodeScope);
        }

        private static TES5SubBranch ConvertSubBranch(TES4SubBranch branch, TES5CodeScope outerCodeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES5ChainedCodeChunkFactory codeChunkFactory, TES5ValueFactory valueFactory)
        {
            TES5LocalScope outerLocalScope = outerCodeScope.LocalScope;
            ITES5Value expression = valueFactory.createValue(branch.Expression, outerCodeScope, globalScope, multipleScriptsScope);
            TES5LocalScope newScope = TES5LocalScopeFactory.createRecursiveScope(outerLocalScope);
            TES5CodeScope newCodeScope = TES5CodeScopeFactory.createCodeScope(newScope);
            TES4CodeChunks branchChunks = branch.CodeChunks;
            if (branchChunks != null)
            {
                foreach (ITES4CodeChunk codeChunk in branchChunks.CodeChunks)
                {
                    TES5CodeChunkCollection codeChunks = codeChunkFactory.createCodeChunk(codeChunk, newCodeScope, globalScope, multipleScriptsScope);
                    if (codeChunks != null)
                    {
                        foreach (ITES5CodeChunk newCodeChunk in codeChunks)
                        {
                            newCodeScope.Add(newCodeChunk);
                        }
                    }
                }
            }

            return new TES5SubBranch(expression, newCodeScope);
        }
    }
}