using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Other;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5FragmentFactory
    {
        private readonly TES5ChainedCodeChunkFactory codeChunkFactory;
        public TES5FragmentFactory(TES5ChainedCodeChunkFactory chainedCodeChunkFactory)
        {
            this.codeChunkFactory = chainedCodeChunkFactory;
        }

        public TES5FunctionCodeBlock CreateFragment(TES5FragmentType fragmentType, string fragmentName, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES4CodeChunks chunks)
        {
            TES5FunctionScope fragmentLocalScope = TES5FragmentFunctionScopeFactory.CreateFromFragmentType(fragmentName, fragmentType);
            TES5FunctionCodeBlock function = new TES5FunctionCodeBlock(fragmentLocalScope, TES5CodeScopeFactory.CreateCodeScopeRoot(fragmentLocalScope), TES5VoidType.Instance);
            foreach (var codeChunk in chunks)
            {
                TES5CodeChunkCollection codeChunks = this.codeChunkFactory.CreateCodeChunk(codeChunk, function.CodeScope, globalScope, multipleScriptsScope);
                foreach (var newCodeChunk in codeChunks)
                {
                    function.AddChunk(newCodeChunk);
                }
            }

            return function;
        }
    }
}