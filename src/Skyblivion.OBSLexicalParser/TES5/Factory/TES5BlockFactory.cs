using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES4.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.Converter;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5BlockFactory
    {
        private TES5ChainedCodeChunkFactory codeChunkFactory;
        private TES5BlockFunctionScopeFactory blockFunctionScopeFactory;
        private TES5CodeScopeFactory codeScopeFactory;
        private TES5AdditionalBlockChangesPass changesPass;
        private TES5LocalScopeFactory localScopeFactory;
        private TES5InitialBlockCodeFactory initialBlockCodeFactory;
        public TES5BlockFactory(TES5ChainedCodeChunkFactory chainedCodeChunkFactory, TES5BlockFunctionScopeFactory blockFunctionScopeFactory, TES5CodeScopeFactory codeScopeFactory, TES5AdditionalBlockChangesPass changesPass, TES5LocalScopeFactory localScopeFactory, TES5InitialBlockCodeFactory initialBlockCodeFactory)
        {
            this.codeChunkFactory = chainedCodeChunkFactory;
            this.blockFunctionScopeFactory = blockFunctionScopeFactory;
            this.codeScopeFactory = codeScopeFactory;
            this.changesPass = changesPass;
            this.localScopeFactory = localScopeFactory;
            this.initialBlockCodeFactory = initialBlockCodeFactory;
        }

        private string mapBlockType(string blockType)
        {
            string newBlockType;
            switch (blockType.ToLower())
            {
                case "gamemode":
                    {
                        newBlockType = "OnUpdate";
                        break;
                    }

                case "onactivate":
                    {
                        newBlockType = "OnActivate";
                        break;
                    }

                case "oninit":
                    {
                        newBlockType = "OnInit";
                        break;
                    }

                case "onsell":
                    {
                        newBlockType = "OnSell";
                        break;
                    }

                case "ondeath":
                    {
                        newBlockType = "OnDeath";
                        break;
                    }

                case "onload":
                    {
                        newBlockType = "OnLoad";
                        break;
                    }

                case "onactorequip":
                    {
                        newBlockType = "OnObjectEquipped";
                        break;
                    }

                case "ontriggeractor":
                    {
                        newBlockType = "OnTriggerEnter";
                        break;
                    }

                case "onadd":
                    {
                        newBlockType = "OnContainerChanged";
                        break;
                    }

                case "onequip":
                    {
                        newBlockType = "OnEquipped";
                        break;
                    }

                case "onunequip":
                    {
                        newBlockType = "OnUnequipped";
                        break;
                    }

                case "ondrop":
                    {
                        newBlockType = "OnContainerChanged";
                        break;
                    }

                case "ontriggermob":
                    {
                        newBlockType = "OnTriggerEnter";
                        break;
                    }

                case "ontrigger":
                    {
                        newBlockType = "OnTrigger";
                        break;
                    }

                case "onhitwith":
                    {
                        newBlockType = "OnHit";
                        break;
                    }

                case "onhit":
                    {
                        newBlockType = "OnHit";
                        break;
                    }

                case "onalarm":
                    {
                        newBlockType = "OnUpdate";
                        break;
                    }

                case "onstartcombat":
                    {
                        newBlockType = "OnCombatStateChanged";
                        break;
                    }

                case "onpackagestart":
                    {
                        newBlockType = "OnPackageStart";
                        break;
                    }

                case "onpackagedone":
                    {
                        newBlockType = "OnPackageEnd";
                        break;
                    }

                case "onpackageend":
                    {
                        newBlockType = "OnPackageEnd";
                        break;
                    }

                case "onpackagechange":
                    {
                        newBlockType = "OnPackageChange";
                        break;
                    }

                case "onmagiceffecthit":
                    {
                        newBlockType = "OnMagicEffectApply";
                        break;
                    }

                case "onreset":
                    {
                        newBlockType = "OnReset";
                        break;
                    }

                case "scripteffectstart":
                    {
                        newBlockType = "OnEffectStart";
                        break;
                    }

                case "scripteffectupdate":
                    {
                        newBlockType = "OnUpdate";
                        break;
                    }

                case "scripteffectfinish":
                    {
                        newBlockType = "OnEffectFinish";
                        break;
                    }

                default:
                    {
                        throw new ConversionException("Cannot find new block type out of " + blockType);
                    }
            }

            return newBlockType;
        }

        public TES5EventCodeBlock createNewBlock(string blockType, TES5FunctionScope functionScope = null)
        {
            if (functionScope == null)
            {
                functionScope = new TES5FunctionScope(blockType);
            }

            TES5EventCodeBlock newBlock = new TES5EventCodeBlock(functionScope, this.codeScopeFactory.createCodeScope(this.localScopeFactory.createRootScope(functionScope)));
            return newBlock;
        }

        public TES5EventBlockList createBlock(TES5MultipleScriptsScope multipleScriptsScope, TES5GlobalScope globalScope, TES4CodeBlock block)
        {
            TES5EventBlockList blockList = new TES5EventBlockList();
            string blockType = block.getBlockType();
            if (blockType.ToLower() == "menumode")
            {
                return blockList;
            }

            string newBlockType = this.mapBlockType(blockType);
            TES5FunctionScope blockFunctionScope = this.blockFunctionScopeFactory.createFromBlockType(newBlockType);
            TES5EventCodeBlock newBlock = this.createNewBlock(newBlockType, blockFunctionScope);
            TES5CodeScope conversionScope = this.initialBlockCodeFactory.addInitialCode(multipleScriptsScope, globalScope, newBlock);
            TES4CodeChunks chunks = block.getChunks();
            if (chunks != null)
            {
                foreach (ITES4CodeChunk codeChunk in chunks.getCodeChunks())
                {
                    TES5CodeChunkCollection codeChunks = this.codeChunkFactory.createCodeChunk(codeChunk, newBlock.getCodeScope(), globalScope, multipleScriptsScope);
                    if (codeChunks != null)
                    {
                        foreach (ITES5CodeChunk newCodeChunk in codeChunks)
                        {
                            conversionScope.add(newCodeChunk);
                        }
                    }
                }

                this.changesPass.modify(block, blockList, newBlock, globalScope, multipleScriptsScope);
                blockList.add(newBlock);
            }

            return blockList;
        }
    }
}