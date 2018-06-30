using Dissect.Extensions.IDictionaryExtensions;
using Skyblivion.OBSLexicalParser.TES4.AST;
using Skyblivion.OBSLexicalParser.TES4.AST.Block;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            TES4BlockList parsedBlockList = script.BlockList;
            Dictionary<string, List<ITES5CodeBlock>> createdBlocks = new Dictionary<string, List<ITES5CodeBlock>>();
            if (parsedBlockList != null)
            {
                foreach (TES4CodeBlock block in parsedBlockList.Blocks)
                {
                    TES5BlockList newBlockList = this.blockFactory.createBlock(block, globalScope, multipleScriptsScope);
                    foreach (ITES5CodeBlock newBlock in newBlockList.Blocks)
                    {
                        createdBlocks.AddNewListIfNotContainsKeyAndAddValueToList(newBlock.BlockName, newBlock);
                    }
                }
            }

            //todo encapsulate it to a different class.
            bool isStandalone = target.getOutputPath().Contains(Path.DirectorySeparatorChar + "Standalone" + Path.DirectorySeparatorChar);
            foreach (var createdBlock in createdBlocks)
            {
                var blockType = createdBlock.Key;
                var blocks = createdBlock.Value;
                if (blocks.Count > 1)
                {
                    List<TES5FunctionCodeBlock> functions = new List<TES5FunctionCodeBlock>();
                    int i = 1;
                    TES5ObjectCallArguments localScopeArguments = null;
                    foreach (TES5CodeBlock block in blocks)
                    {
                        TES5FunctionScope newFunctionScope = new TES5FunctionScope(blockType+"_"+i.ToString());
                        foreach (var variable in block.FunctionScope.Variables.Values)
                        {
                            newFunctionScope.AddVariable(variable);
                        }

                        TES5FunctionCodeBlock function = new TES5FunctionCodeBlock(newFunctionScope, block.CodeScope, new TES5VoidType(), isStandalone);
                        functions.Add(function);
                        if (localScopeArguments == null)
                        {
                            localScopeArguments = new TES5ObjectCallArguments();
                            foreach (var variable in block.FunctionScope.Variables.Values)
                            {
                                localScopeArguments.Add(TES5ReferenceFactory.CreateReferenceToVariable(variable));
                            }
                        }

                        ++i;
                    }

                    //Create the proxy block.
                    ITES5CodeBlock lastBlock = blocks.Last();
                    TES5EventCodeBlock proxyBlock = TES5BlockFactory.CreateEventCodeBlock(blockType,
                        /*
                        //WTM:  Change:  block was used below, but block is out of scope.  The PHP must have been using the last defined block from above.
                        //WTM:  Change:  PHP called "clone" below, but I'm not sure if this is necessary or would even operate the same in C#.
                        */
                        lastBlock.FunctionScope);
                    foreach (var function in functions)
                    {
                        blockList.Add(function);
                        TES5ObjectCall functionCall = this.objectCallFactory.CreateObjectCall(TES5ReferenceFactory.CreateReferenceToSelf(globalScope), function.BlockName, multipleScriptsScope, localScopeArguments, false // hacky.
                        );
                        proxyBlock.AddChunk(functionCall);
                    }

                    blockList.Add(proxyBlock);
                }
                else
                {
                    ITES5CodeBlock block = blocks[0];
                    blockList.Add(block);
                }
            }

            TES5Target result = new TES5Target(new TES5Script(globalScope, blockList), target.getOutputPath());
            return result;
        }
    }
}