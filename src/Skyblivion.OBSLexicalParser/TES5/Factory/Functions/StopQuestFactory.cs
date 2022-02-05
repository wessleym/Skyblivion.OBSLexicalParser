using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class StopQuestFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        private readonly TES5ReferenceFactory referenceFactory;
        public StopQuestFactory(TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.referenceFactory = referenceFactory;
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
            TES5CodeChunkCollection codeChunks = new TES5CodeChunkCollection();
            codeChunks.Add(this.objectCallFactory.CreateObjectCall(newCalledOn, "Stop", this.objectCallArgumentsFactory.CreateArgumentList(functionArguments, codeScope, globalScope, multipleScriptsScope)));
            if (questName == "FGInterimConversation" || questName == "ArenaIC" || questName == "ArenaICGrandChampion" || questName == "ArenaAggression" || questName == "ArenaAnnouncer" || questName == "ArenaDisqualification" || questName == "Arena")
            {
                codeChunks.Add(this.objectCallFactory.CreateObjectCall(newCalledOn, "PrepareForReinitializing", new TES5ObjectCallArguments()));
            }
            return codeChunks;
        }
    }
}