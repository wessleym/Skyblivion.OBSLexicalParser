using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5ReturnFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        public TES5ReturnFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public TES5CodeChunkCollection CreateCodeChunkCollection(TES5FunctionScope functionScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5CodeChunkCollection collection = new TES5CodeChunkCollection();
            if (functionScope.BlockName == "OnUpdate")
            {
                TES5ObjectCall function = this.objectCallFactory.CreateRegisterForSingleUpdate(globalScope, multipleScriptsScope);
                collection.Add(function);
            }
            collection.Add(new TES5Return());
            return collection;
        }
    }
}