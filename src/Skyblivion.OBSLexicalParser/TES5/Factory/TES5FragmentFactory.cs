using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Converter;
using Skyblivion.OBSLexicalParser.TES5.Other;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5FragmentFactory
    {
        private readonly TES5ChainedCodeChunkFactory codeChunkFactory;
        private readonly TES5AdditionalBlockChangesPass changesPass;
        public TES5FragmentFactory(TES5ChainedCodeChunkFactory chainedCodeChunkFactory, TES5AdditionalBlockChangesPass changesPass)
        {
            this.codeChunkFactory = chainedCodeChunkFactory;
            this.changesPass = changesPass;
        }

        public TES5FunctionCodeBlock CreateFragment(TES5FragmentType fragmentType, string fragmentName, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES4CodeChunks chunks)
        {
            TES5FunctionScope fragmentLocalScope = TES5FragmentFunctionScopeFactory.CreateFromFragmentType(fragmentName, fragmentType);
            TES5FunctionCodeBlock function = new TES5FunctionCodeBlock(fragmentLocalScope, TES5CodeScopeFactory.CreateCodeScopeRoot(fragmentLocalScope), new TES5VoidType());
            foreach (var codeChunk in chunks)
            {
                TES5CodeChunkCollection codeChunks = this.codeChunkFactory.createCodeChunk(codeChunk, function.CodeScope, globalScope, multipleScriptsScope);
                if (codeChunks != null)
                {
                    foreach (var newCodeChunk in codeChunks)
                    {
                        function.AddChunk(newCodeChunk);
                    }
                }
            }

            return function;
        }
    }
}