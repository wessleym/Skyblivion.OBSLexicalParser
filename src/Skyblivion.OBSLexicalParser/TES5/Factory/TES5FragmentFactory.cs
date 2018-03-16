using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Converter;
using Skyblivion.OBSLexicalParser.TES5.Other;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5FragmentFactory
    {
        private TES5ChainedCodeChunkFactory codeChunkFactory;
        private TES5FragmentFunctionScopeFactory fragmentFunctionScopeFactory;
        private TES5CodeScopeFactory codeScopeFactory;
        private TES5AdditionalBlockChangesPass changesPass;
        private TES5LocalScopeFactory localScopeFactory;
        public TES5FragmentFactory(TES5ChainedCodeChunkFactory chainedCodeChunkFactory, TES5FragmentFunctionScopeFactory fragmentLocalScopeFactory, TES5CodeScopeFactory codeScopeFactory, TES5AdditionalBlockChangesPass changesPass, TES5LocalScopeFactory localScopeFactory)
        {
            this.codeChunkFactory = chainedCodeChunkFactory;
            this.fragmentFunctionScopeFactory = fragmentLocalScopeFactory;
            this.codeScopeFactory = codeScopeFactory;
            this.changesPass = changesPass;
            this.localScopeFactory = localScopeFactory;
        }

        public TES5FunctionCodeBlock createFragment(TES5FragmentType fragmentType, string fragmentName, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES4CodeChunks chunks)
        {
            TES5FunctionScope fragmentLocalScope = this.fragmentFunctionScopeFactory.createFromFragmentType(fragmentName, fragmentType);
            TES5FunctionCodeBlock function = new TES5FunctionCodeBlock(new TES5VoidType(), fragmentLocalScope, this.codeScopeFactory.createCodeScope(this.localScopeFactory.createRootScope(fragmentLocalScope)));
            foreach (var codeChunk in chunks.getCodeChunks())
            {
                TES5CodeChunkCollection codeChunks = this.codeChunkFactory.createCodeChunk(codeChunk, function.getCodeScope(), globalScope, multipleScriptsScope);
                if (codeChunks != null)
                {
                    foreach (var newCodeChunk in codeChunks)
                    {
                        function.addChunk(newCodeChunk);
                    }
                }
            }

            return function;
        }
    }
}