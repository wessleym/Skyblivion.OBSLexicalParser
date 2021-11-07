using Skyblivion.OBSLexicalParser.TES4.AST;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST;
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
            TES5FunctionCodeBlock function = new TES5FunctionCodeBlock(fragmentLocalScope, TES5CodeScopeFactory.CreateCodeScopeRoot(fragmentLocalScope), TES5VoidType.Instance, false, fragmentType == TES5FragmentType.T_QF || fragmentType == TES5FragmentType.T_TIF);
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

        private TES5FunctionCodeBlock CreateFragment(TES5FragmentType fragmentType, int stageID, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES4CodeChunks chunks)
        {
            string fragmentName = GetFragmentName(stageID);
            return CreateFragment(fragmentType, fragmentName, globalScope, multipleScriptsScope, chunks);
        }

        public TES5Target Convert(TES5FragmentType fragmentType, TES4FragmentTarget fragmentTarget, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5FunctionCodeBlock fragment = CreateFragment(fragmentType, 0, globalScope, multipleScriptsScope, fragmentTarget.CodeChunks);
            TES5BlockList blockList = new TES5BlockList() { fragment };
            TES5Script script = new TES5Script(globalScope, blockList, true);
            TES5Target target = new TES5Target(script, fragmentTarget.OutputPath);
            return target;
        }

        public static string GetFragmentName(int stageIndex, int logIndex = 0)
        {
            return "Fragment_" + stageIndex.ToString() + (logIndex != 0 ? "_" + logIndex : "");
        }
    }
}