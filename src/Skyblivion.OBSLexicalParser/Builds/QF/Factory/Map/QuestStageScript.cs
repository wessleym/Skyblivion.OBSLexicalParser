using Skyblivion.OBSLexicalParser.TES5.AST;

namespace Skyblivion.OBSLexicalParser.Builds.QF.Factory.Map
{
    class QuestStageScript//WTM:  Added:  For ordering stages in PSC output
    {
        public TES5Script Script { get; }
        public int Stage { get; }
        public int LogIndex { get; }
        public QuestStageScript(TES5Script script, int stage, int logIndex)
        {
            this.Script = script;
            this.Stage = stage;
            this.LogIndex = logIndex;
        }
    }
}