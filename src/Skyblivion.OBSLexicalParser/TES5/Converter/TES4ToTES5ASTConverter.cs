using Skyblivion.OBSLexicalParser.TES4.AST;
using Skyblivion.OBSLexicalParser.TES4.AST.Block;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Converter
{
    class TES4ToTES5ASTConverter
    {
        /*
             * Oblivion binary data analyzer.
        */
        private ESMAnalyzer esmAnalyzer;
        private TES5BlockFactory blockFactory;
        private TES5ObjectCallFactory objectCallFactory;
        private TES5ReferenceFactory referenceFactory;
        public TES4ToTES5ASTConverter(ESMAnalyzer ESMAnalyzer, TES5BlockFactory blockFactory, TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory)
        {
            this.esmAnalyzer = ESMAnalyzer;
            this.blockFactory = blockFactory;
            this.objectCallFactory = objectCallFactory;
            this.referenceFactory = referenceFactory;
        }

        /*
        *  The script to be converted
        *  The script"s global scope
        *  The scope under which we"re converting
        * 
        * @throws ConversionException
        */
        public TES5Target convert(TES4Target target, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4Script script = target.getScript();
            TES5BlockList blockList = new TES5BlockList();
            TES4BlockList parsedBlockList = script.getBlockList();
            Dictionary<string, List<TES5EventCodeBlock>> createdBlocks = new Dictionary<string, List<TES5EventCodeBlock>>();
            if (parsedBlockList != null)
            {
                foreach (TES4CodeBlock block in parsedBlockList.getBlocks())
                {
                    TES5EventBlockList newBlockList = this.blockFactory.createBlock(multipleScriptsScope, globalScope, block);
                    foreach (TES5EventCodeBlock newBlock in newBlockList.getBlocks())
                    {
                        if (!createdBlocks.ContainsKey(newBlock.getBlockType()))
                        {
                            createdBlocks[newBlock.getBlockType()] = new List<TES5EventCodeBlock>();
                        }

                        createdBlocks[newBlock.getBlockType()].Add(newBlock);
                    }
                }
            }

            //todo encapsulate it to a different class.
            foreach (var createdBlock in createdBlocks)
            {
                var blockType = createdBlock.Key;
                var blocks = createdBlock.Value;
                if (blocks.Count > 1)
                {
                    List<TES5FunctionCodeBlock> functions = new List<TES5FunctionCodeBlock>();
                    int i = 1;
                    TES5ObjectCallArguments localScopeArguments = null;
                    foreach (TES5EventCodeBlock block in blocks)
                    {
                        TES5FunctionScope newFunctionScope = new TES5FunctionScope(blockType+"_"+i.ToString());
                        foreach (var variable in block.getFunctionScope().getVariables())
                        {
                            newFunctionScope.addVariable(variable.Value);
                        }

                        TES5FunctionCodeBlock function = new TES5FunctionCodeBlock(null, newFunctionScope, block.getCodeScope());
                        functions.Add(function);
                        if (localScopeArguments == null)
                        {
                            localScopeArguments = new TES5ObjectCallArguments();
                            foreach (var kvp2 in block.getFunctionScope().getVariables())
                            {
                                localScopeArguments.add(this.referenceFactory.createReferenceToVariable(kvp2.Value));
                            }
                        }

                        ++i;
                    }

                    //Create the proxy block.
                    TES5EventCodeBlock proxyBlock = this.blockFactory.createNewBlock(blockType,
                        /*
                        //WTM:  Change:  block was used below, but block is out of scope.  I'm omitting the argument instead.
                        clone $block->getFunctionScope()
                        */
                        null);
                    foreach (var function in functions)
                    {
                        blockList.add(function);
                        TES5ObjectCall functionCall = this.objectCallFactory.createObjectCall(this.referenceFactory.createReferenceToSelf(globalScope), function.getFunctionName(), multipleScriptsScope, localScopeArguments, false // hacky.
                        );
                        proxyBlock.addChunk(functionCall);
                    }

                    blockList.add(proxyBlock);
                }
                else
                {
                    TES5EventCodeBlock block = blocks[0];
                    blockList.add(block);
                }
            }

            TES5Target result = new TES5Target(new TES5Script(globalScope, blockList), target.getOutputPath());
            return result;
        }
    }
}