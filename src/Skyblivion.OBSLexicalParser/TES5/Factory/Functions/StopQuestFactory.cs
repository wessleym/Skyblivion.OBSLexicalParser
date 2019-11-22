using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Service;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class StopQuestFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly ESMAnalyzer analyzer;
        private readonly TES5ObjectPropertyFactory objectPropertyFactory;
        private readonly TES5TypeInferencer typeInferencer;
        private readonly MetadataLogService metadataLogService;
        private readonly TES5ValueFactory valueFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public StopQuestFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer analyzer,TES5TypeInferencer typeInferencer, MetadataLogService metadataLogService)
        {
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.valueFactory = valueFactory;
            this.referenceFactory = referenceFactory;
            this.analyzer = analyzer;
            this.objectPropertyFactory = objectPropertyFactory;
            this.typeInferencer = typeInferencer;
            this.metadataLogService = metadataLogService;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4FunctionArguments functionArguments = function.Arguments;
            string questName = functionArguments.Pop(0).StringValue;
            ITES5Referencer newCalledOn = this.referenceFactory.CreateReadReference(questName, globalScope, multipleScriptsScope, localScope);
            /*
             * Basically, there are some ugly mechanics in Oblivion.
             * Two quests ( FGInterimConversation and Arena* quest group ) are repeadetely started and stopped
             * However, Skyrim does not support this - once 0x13A byte is marked in a TESQuest, it won"t allow
             * to be started again. Hence, we need to call a Papyrus endpoint to stop the quest and
             * reset this field, and be able to reset the quest completely.
             */
            if (questName == "FGInterimConversation" || questName == "ArenaIC" || questName == "ArenaICGrandChampion" || questName == "ArenaAggression" || questName == "ArenaAnnouncer" || questName == "ArenaDisqualification" || questName == "Arena")
            {
                return this.objectCallFactory.CreateObjectCall(newCalledOn, "PrepareForReinitializing", new TES5ObjectCallArguments());
            }
            else
            {
                return this.objectCallFactory.CreateObjectCall(newCalledOn, "Stop", this.objectCallArgumentsFactory.CreateArgumentList(functionArguments, codeScope, globalScope, multipleScriptsScope));
            }
        }
    }
}