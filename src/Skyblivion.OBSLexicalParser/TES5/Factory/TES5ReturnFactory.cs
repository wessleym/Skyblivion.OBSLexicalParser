using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Converter;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5ReturnFactory
    {
        private TES5ObjectCallFactory objectCallFactory;
        private TES5ReferenceFactory referenceFactory;
        private TES5BlockFunctionScopeFactory localScopeFactory;
        public TES5ReturnFactory(TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory, TES5BlockFunctionScopeFactory localScopeFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.referenceFactory = referenceFactory;
            this.localScopeFactory = localScopeFactory;
        }

        public TES5CodeChunkCollection createCodeChunk(TES5FunctionScope functionScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5CodeChunkCollection collection = new TES5CodeChunkCollection();
            /*
             * @todo - Rework the block types so that the information about the block type and its name is not carried within one field
             */
            if (functionScope.getBlockName() == "OnUpdate")
            {
                TES5ObjectCallArguments args = new TES5ObjectCallArguments();
                args.add(new TES5Float(TES5AdditionalBlockChangesPass.ON_UPDATE_TICK));
                TES5ObjectCall function = this.objectCallFactory.createObjectCall(this.referenceFactory.createReferenceToSelf(globalScope), "RegisterForSingleUpdate", multipleScriptsScope, args);
                collection.add(function);
            }

            collection.add(new TES5Return());
            return collection;
        }
    }
}