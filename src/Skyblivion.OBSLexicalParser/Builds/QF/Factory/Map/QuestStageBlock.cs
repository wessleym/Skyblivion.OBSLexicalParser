using Skyblivion.OBSLexicalParser.TES5.AST.Block;

namespace Skyblivion.OBSLexicalParser.Builds.QF.Factory.Map
{
    class QuestStageBlock
    {
        public readonly int StageID;
        public readonly int LogIndex;
        public readonly ITES5CodeBlock CodeBlock;
        public QuestStageBlock(int stageID, int logIndex, ITES5CodeBlock codeBlock)
        {
            StageID = stageID;
            LogIndex = logIndex;
            CodeBlock = codeBlock;
        }
    }
}
