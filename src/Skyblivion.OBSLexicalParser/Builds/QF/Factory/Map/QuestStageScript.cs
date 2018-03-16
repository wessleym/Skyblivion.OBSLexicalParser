using Skyblivion.OBSLexicalParser.TES5.AST;

namespace Skyblivion.OBSLexicalParser.Builds.QF.Factory.Map
{
    class QuestStageScript
    {
        private TES5Target script;
        private int stage;
        private int logIndex;
        public QuestStageScript(TES5Target script, int stage, int logIndex)
        {
            this.script = script;
            this.stage = stage;
            this.logIndex = logIndex;
        }

        public TES5Target getScript()
        {
            return this.script;
        }

        public int getStage()
        {
            return this.stage;
        }

        public int getLogIndex()
        {
            return this.logIndex;
        }
    }
}