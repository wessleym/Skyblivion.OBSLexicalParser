using Skyblivion.OBSLexicalParser.TES5.AST;

namespace Skyblivion.OBSLexicalParser.Builds.QF.Factory.Map
{
    class QuestStageScript
    {
        public TES5Target Script { get; private set; }
        public int Stage { get; private set; }
        public int LogIndex { get; private set; }
        public QuestStageScript(TES5Target script, int stage, int logIndex)
        {
            this.Script = script;
            this.Stage = stage;
            this.LogIndex = logIndex;
        }
    }
}