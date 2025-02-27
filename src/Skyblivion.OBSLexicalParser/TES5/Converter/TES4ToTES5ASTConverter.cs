using Dissect.Extensions;
using Skyblivion.OBSLexicalParser.TES4.AST;
using Skyblivion.OBSLexicalParser.TES4.AST.Block;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Factory.Functions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Converter
{
    class TES4ToTES5ASTConverter
    {
        /*
             * Oblivion binary data analyzer.
        */
        private readonly TES5BlockFactory blockFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        public TES4ToTES5ASTConverter(TES5BlockFactory blockFactory, TES5ObjectCallFactory objectCallFactory)
        {
            this.blockFactory = blockFactory;
            this.objectCallFactory = objectCallFactory;
        }

        /*
        *  The script to be converted
        *  The script's global scope
        *  The scope under which we're converting
        * 
        * @throws ConversionException
        */
        public TES5Target Convert(TES4Target target, bool isStandalone, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4Script script = target.Script;
            Dictionary<string, List<ITES5CodeBlock>> createdBlocks = new Dictionary<string, List<ITES5CodeBlock>>();
            TES5EventCodeBlock? onUpdateBlockOfNonQuestOrAME = null;
            List<TES5Comment> waitingComments = new List<TES5Comment>();//Collects comments of the upcoming block.
            Dictionary<string, List<TES5Comment>> blockListComments = new Dictionary<string, List<TES5Comment>>();
            foreach (ITES4CodeBlockOrComment blockOrComment in script.BlockList)
            {
                TES4CodeBlock? block = blockOrComment as TES4CodeBlock;
                if (block != null)
                {
                    List<ITES5CodeBlock> newBlocks = this.blockFactory.CreateBlock(block, globalScope, multipleScriptsScope, ref onUpdateBlockOfNonQuestOrAME, waitingComments);
                    if (newBlocks.Any())
                    {
                        foreach (ITES5CodeBlock newBlock in newBlocks)
                        {
                            createdBlocks.AddNewListIfNotContainsKeyAndAddValueToList(newBlock.BlockName, newBlock);
                        }
                        if (waitingComments.Any())
                        {
                            blockListComments.AddNewListIfNotContainsKeyAndAddValueToList(newBlocks[0].BlockName, waitingComments);
                        }
                    }
                    waitingComments.Clear();
                    continue;
                }
                TES4Comment? comment = blockOrComment as TES4Comment;
                if (comment != null)
                {
                    waitingComments.Add(TES5CommentFactory.Construct(comment));
                    continue;
                }
                throw new InvalidOperationException("Unhandled " + nameof(blockOrComment) + " type:  " + blockOrComment.GetType().FullName);
            }

            List<ITES5CodeBlock> blockList = new List<ITES5CodeBlock>();
            foreach (var createdBlock in createdBlocks)
            {
                var blockType = createdBlock.Key;
                var blocks = createdBlock.Value;
                if (blocks.Count > 1)
                {
                    foreach (TES5CodeBlock block in CombineRepeatedEventCodeBlockNames(blocks, blockType, isStandalone, globalScope))
                    {
                        blockList.Add(block);
                    }
                }
                else
                {
                    ITES5CodeBlock block = blocks[0];
                    blockList.Add(block);
                }
            }

            ITES5Outputtable[] blocksWithComments = blockList
                .SelectMany(block =>
                {
                    List<ITES5Outputtable> blocks = new List<ITES5Outputtable>();
                    List<TES5Comment>? comments;
                    if (blockListComments.TryGetValue(block.BlockName, out comments))
                    {
                        blocks.AddRange(comments);
                    }
                    blocks.Add(block);
                    return blocks;
                })
                .Concat(waitingComments)//Concat remaining comments at the bottom of the file
                .ToArray();
            TES5Target result = new TES5Target(new TES5Script(globalScope, blockList, blocksWithComments, false), target.OutputPath);
            return result;
        }

        private IEnumerable<TES5CodeBlock> CombineRepeatedEventCodeBlockNames(List<ITES5CodeBlock> blocks, string blockType, bool isStandalone, TES5GlobalScope globalScope)
        {
            ITES5CodeBlock[] nonEvents = blocks.Where(b => !(b is TES5EventCodeBlock)).ToArray();
            if (nonEvents.Any())
            {
                throw new ConversionException("Some non-event code blocks were present in " + nameof(CombineRepeatedEventCodeBlockNames) + ":  " + string.Join("; ", nonEvents.Select(b => b.GetType().FullName)));
            }
            TES5EventCodeBlock[] eventBlocks = blocks.Cast<TES5EventCodeBlock>().ToArray();
            List<TES5FunctionCodeBlock> functions = new List<TES5FunctionCodeBlock>();
            TES5ObjectCallArguments? localScopeArguments = null;
            for (int i = 0; i < eventBlocks.Length; i++)
            {
                ITES5CodeBlock block = eventBlocks[i];
                TES5FunctionScope newFunctionScope = new TES5FunctionScope(blockType + "_" + (i + 1).ToString());
                foreach (var parameter in block.FunctionScope.GetParameters())
                {
                    newFunctionScope.AddParameter(parameter);
                }

                TES5FunctionCodeBlock function = new TES5FunctionCodeBlock(newFunctionScope, block.CodeScope, TES5VoidType.Instance, isStandalone, false);
                functions.Add(function);
                if (localScopeArguments == null)
                {
                    localScopeArguments = new TES5ObjectCallArguments();
                    foreach (var parameter in block.FunctionScope.GetParameters())
                    {
                        localScopeArguments.Add(TES5ReferenceFactory.CreateReferenceToVariableOrProperty(parameter));
                    }
                }
            }

            if (functions.Any())
            {
                //Create the proxy block.
                ITES5CodeBlock lastBlock = blocks.Last();
                TES5EventCodeBlock proxyBlock = TES5BlockFactory.CreateEventCodeBlock(
                    //WTM:  Change:  block was used below, but block is out of scope.  The PHP must have been using the last defined block from above.
                    //WTM:  Change:  PHP called "clone" below, but I'm not sure if this is necessary or would even operate the same in C#.
                    lastBlock.FunctionScope, globalScope);
                foreach (var function in functions)
                {
                    yield return function;
                    TES5ObjectCall functionCall = this.objectCallFactory.CreateObjectCall(TES5ReferenceFactory.CreateReferenceToSelf(globalScope), function.BlockName, localScopeArguments, inference: false// hacky.
                        );
                    proxyBlock.AddChunk(functionCall);
                }

                yield return proxyBlock;
            }

            /*
            //This was not ultimately necessary.
            IGrouping<string, TES5StateCodeBlock>[] states = blocks.OfType<TES5StateCodeBlock>().ToLookup(b => b.BlockName).ToArray();
            List<TES5CodeBlock> consolidatedStateCodeBlocks = states.Where(b => b.Count() == 1).SelectMany(b => b.Select(b2 => (TES5CodeBlock)b2).ToArray()).ToList();
            IGrouping<string, TES5StateCodeBlock>[] repeatedStateCodeBlocks = states.Where(b => b.Count() > 1).ToArray();
            for (int i = 0; i < repeatedStateCodeBlocks.Length; i++)
            {
                IGrouping<string, TES5StateCodeBlock> groupsWithSameName = repeatedStateCodeBlocks[i];
                TES5StateCodeBlock firstGroup = groupsWithSameName.First();
                foreach (var otherGroup in groupsWithSameName.Skip(1))
                {
                    foreach (ITES5CodeChunk chunk in otherGroup.CodeScope.CodeChunks)
                    {
                        firstGroup.AddChunk(chunk);
                    }
                }
                if (firstGroup.CodeBlocks.Blocks.Any())
                {
                    TES5CodeBlock[] reducedBlocks = CombineRepeatedCodeBlockNames(firstGroup.CodeBlocks.Blocks, blockType, isStandalone, globalScope, multipleScriptsScope).ToArray();
                    firstGroup.CodeBlocks.Blocks.Clear();
                    firstGroup.CodeBlocks.Blocks.AddRange(reducedBlocks);
                }
                consolidatedStateCodeBlocks.Add(firstGroup);
            }
            foreach(TES5CodeBlock codeBlock in consolidatedStateCodeBlocks)
            {
                yield return codeBlock;
            }
            */
        }
    }
}