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
            TES5SubBranch mainBranch = ConvertSubBranch(chunk.MainBranch, codeScope, globalScope, multipleScriptsScope, codeChunkFactory, valueFactory);
            TES4SubBranchList? branchList = chunk.ElseifBranches;
            TES5SubBranchList? convertedElseIfBranches = null;
            if (branchList != null)
            {
                convertedElseIfBranches = new TES5SubBranchList();
                foreach (TES4SubBranch subBranch in branchList.Branches)
                {
                    convertedElseIfBranches.Add(ConvertSubBranch(subBranch, codeScope, globalScope, multipleScriptsScope, codeChunkFactory, valueFactory));
                }
            }

            TES4ElseSubBranch? elseBranch = chunk.ElseBranch;
            TES5ElseSubBranch? convertedElseBranch = null;
            if (elseBranch != null)
            {
                convertedElseBranch = ConvertElseBranch(elseBranch, codeScope, globalScope, multipleScriptsScope, codeChunkFactory);
            }

            return new TES5CodeChunkCollection() { new TES5Branch(mainBranch, convertedElseIfBranches, convertedElseBranch) };
        }

        public static TES5Branch CreateSimpleBranch(ITES5Expression expression, TES5LocalScope parentScope)
        {
            return new TES5Branch(CreateSubBranch(expression, parentScope));
        }

        public static TES5SubBranch CreateSubBranch(ITES5Expression expression, TES5LocalScope parentScope)
        {
            return new TES5SubBranch(expression, TES5CodeScopeFactory.CreateCodeScopeRecursive(parentScope));
        }

        private static TES5ElseSubBranch ConvertElseBranch(TES4ElseSubBranch branch, TES5CodeScope outerCodeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES5ChainedCodeChunkFactory codeChunkFactory)
        {
            TES5CodeScope newCodeScope = TES5CodeScopeFactory.CreateCodeScopeRecursive(outerCodeScope.LocalScope);
            TES4CodeChunks? branchChunks = branch.CodeChunks;
            if (branchChunks != null)
            {
                foreach (ITES4CodeChunk codeChunk in branchChunks)
                {
                    TES5CodeChunkCollection codeChunks = codeChunkFactory.CreateCodeChunk(codeChunk, newCodeScope, globalScope, multipleScriptsScope);
                    foreach (ITES5CodeChunk newCodeChunk in codeChunks)
                    {
                        newCodeScope.AddChunk(newCodeChunk);
                    }
                }
            }
            return new TES5ElseSubBranch(newCodeScope);
        }

        private static TES5SubBranch ConvertSubBranch(TES4SubBranch branch, TES5CodeScope outerCodeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES5ChainedCodeChunkFactory codeChunkFactory, TES5ValueFactory valueFactory)
        {
            ITES5Value expression = valueFactory.CreateValue(branch.Expression, outerCodeScope, globalScope, multipleScriptsScope);
            TES5CodeScope newCodeScope = TES5CodeScopeFactory.CreateCodeScopeRecursive(outerCodeScope.LocalScope);
            TES4CodeChunks? branchChunks = branch.CodeChunks;
            if (branchChunks != null)
            {
                foreach (ITES4CodeChunk codeChunk in branchChunks)
                {
                    TES5CodeChunkCollection codeChunks = codeChunkFactory.CreateCodeChunk(codeChunk, newCodeScope, globalScope, multipleScriptsScope);
                    foreach (ITES5CodeChunk newCodeChunk in codeChunks)
                    {
                        newCodeScope.AddChunk(newCodeChunk);
                    }
                }
            }
            return new TES5SubBranch(expression, newCodeScope);
        }
    }
}