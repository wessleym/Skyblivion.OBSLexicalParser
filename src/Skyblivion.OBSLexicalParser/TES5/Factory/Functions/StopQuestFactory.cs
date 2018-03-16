using Ormin.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Service;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class StopQuestFactory : IFunctionFactory
    {
        private TES5ReferenceFactory referenceFactory;
        private TES5ExpressionFactory expressionFactory;
        private TES5VariableAssignationFactory assignationFactory;
        private ESMAnalyzer analyzer;
        private TES5ObjectPropertyFactory objectPropertyFactory;
        private TES5PrimitiveValueFactory primitiveValueFactory;
        private TES5TypeInferencer typeInferencer;
        private MetadataLogService metadataLogService;
        private TES5ValueFactory valueFactory;
        private TES5ObjectCallFactory objectCallFactory;
        private TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public StopQuestFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, TES5ExpressionFactory expressionFactory, TES5VariableAssignationFactory assignationFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer analyzer, TES5PrimitiveValueFactory primitiveValueFactory, TES5TypeInferencer typeInferencer, MetadataLogService metadataLogService)
        {
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.valueFactory = valueFactory;
            this.referenceFactory = referenceFactory;
            this.expressionFactory = expressionFactory;
            this.analyzer = analyzer;
            this.assignationFactory = assignationFactory;
            this.objectPropertyFactory = objectPropertyFactory;
            this.primitiveValueFactory = primitiveValueFactory;
            this.typeInferencer = typeInferencer;
            this.metadataLogService = metadataLogService;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5CodeChunk convertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.getLocalScope();
            TES4FunctionArguments functionArguments = function.getArguments();
            string questName = (string)functionArguments.popValue(0).getData();
            ITES5Referencer newCalledOn = this.referenceFactory.createReadReference(questName, globalScope, multipleScriptsScope, localScope);
            /*
             * Basically, there are some ugly mechanics in Oblivion.
             * Two quests ( FGInterimConversation and Arena* quest group ) are repeadetely started and stopped
             * However, Skyrim does not support this - once 0x13A byte is marked in a TESQuest, it won"t allow
             * to be started again. Hence, we need to call a Papyrus endpoint to stop the quest and
             * reset this field, and be able to reset the quest completely.
             */
            if (questName == "FGInterimConversation" || questName == "ArenaIC" || questName == "ArenaICGrandChampion" || questName == "ArenaAggression" || questName == "ArenaAnnouncer" || questName == "ArenaDisqualification" || questName == "Arena")
            {
                return this.objectCallFactory.createObjectCall(newCalledOn, "PrepareForReinitializing", multipleScriptsScope, new TES5ObjectCallArguments());
            }
            else
            {
                return this.objectCallFactory.createObjectCall(newCalledOn, "Stop", multipleScriptsScope, this.objectCallArgumentsFactory.createArgumentList(functionArguments, codeScope, globalScope, multipleScriptsScope));
            }
        }
    }
}