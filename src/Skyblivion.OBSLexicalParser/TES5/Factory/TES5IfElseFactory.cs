using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.Code.Branch;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    static class TES5IfElseFactory
    {
        private static TES5IfElseClause CreateIfElseClause(ITES5Value condition, TES5CodeScope codeScope)
        {
            return new TES5IfElseClause(condition, codeScope);
        }
        private static TES5IfElseClause CreateIfElseClause(ITES5Value expression, TES5LocalScope parentScope)
        {
            return CreateIfElseClause(expression, TES5CodeScopeFactory.CreateCodeScopeRecursive(parentScope));
        }

        private static TES5IfElseChain CreateIfElseChain(TES5IfElseClause ifClause, List<TES5IfElseClause>? elseIfClauses = null, TES5ElseClause? elseClause = null)
        {
            return new TES5IfElseChain(ifClause, elseIfClauses: elseIfClauses, elseClause: elseClause);
        }
        public static TES5IfElseChain CreateIfElseChain(ITES5Value ifCondition, TES5LocalScope parentScope)
        {
            return CreateIfElseChain(CreateIfElseClause(ifCondition, parentScope));
        }
        public static TES5IfElseChain CreateIfElseChain(ITES5Value ifCondition, TES5CodeScope codeScope)
        {
            return CreateIfElseChain(CreateIfElseClause(ifCondition, codeScope));
        }

        public static TES5CodeChunkCollection CreateCodeChunk(TES4IfElseChain ifElseChain, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES5ChainedCodeChunkFactory codeChunkFactory, TES5ValueFactory valueFactory)
        {
            TES5IfElseClause ifClause = ConvertIfElseClause(ifElseChain.IfClause, codeScope, globalScope, multipleScriptsScope, codeChunkFactory, valueFactory);
            List<TES4IfElseClause>? elseIfClauses = ifElseChain.ElseIfClauses;
            List<TES5IfElseClause>? convertedElseIfClauses = null;
            if (elseIfClauses != null)
            {
                convertedElseIfClauses = new List<TES5IfElseClause>();
                foreach (TES4IfElseClause elseIfClause in elseIfClauses)
                {
                    convertedElseIfClauses.Add(ConvertIfElseClause(elseIfClause, codeScope, globalScope, multipleScriptsScope, codeChunkFactory, valueFactory));
                }
            }

            TES4ElseClause? elseClause = ifElseChain.ElseClause;
            TES5ElseClause? convertedElseClause = null;
            if (elseClause != null)
            {
                convertedElseClause = ConvertElseClause(elseClause, codeScope, globalScope, multipleScriptsScope, codeChunkFactory);
            }

            return new TES5CodeChunkCollection() { CreateIfElseChain(ifClause, elseIfClauses: convertedElseIfClauses, elseClause: convertedElseClause) };
        }

        private static TES5ElseClause ConvertElseClause(TES4ElseClause elseClause, TES5CodeScope outerCodeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES5ChainedCodeChunkFactory codeChunkFactory)
        {
            TES5CodeScope newCodeScope = TES5CodeScopeFactory.CreateCodeScopeRecursive(outerCodeScope.LocalScope);
            foreach (ITES4CodeChunk codeChunk in elseClause.CodeChunks.Chunks)
            {
                TES5CodeChunkCollection codeChunks = codeChunkFactory.CreateCodeChunk(codeChunk, newCodeScope, globalScope, multipleScriptsScope);
                foreach (ITES5CodeChunk newCodeChunk in codeChunks)
                {
                    newCodeScope.AddChunk(newCodeChunk);
                }
            }
            return new TES5ElseClause(newCodeScope);
        }

        private static TES5IfElseClause ConvertIfElseClause(TES4IfElseClause ifElseClause, TES5CodeScope outerCodeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES5ChainedCodeChunkFactory codeChunkFactory, TES5ValueFactory valueFactory)
        {
            ITES5Value ifCondition = valueFactory.CreateValue(ifElseClause.Condition, outerCodeScope, globalScope, multipleScriptsScope);
            TES5CodeScope newCodeScope = TES5CodeScopeFactory.CreateCodeScopeRecursive(outerCodeScope.LocalScope);
            foreach (ITES4CodeChunk codeChunk in ifElseClause.CodeChunks.Chunks)
            {
                TES5CodeChunkCollection codeChunks = codeChunkFactory.CreateCodeChunk(codeChunk, newCodeScope, globalScope, multipleScriptsScope);
                foreach (ITES5CodeChunk newCodeChunk in codeChunks)
                {
                    newCodeScope.AddChunk(newCodeChunk);
                }
            }
            return CreateIfElseClause(ifCondition, newCodeScope);
        }
    }
}