namespace Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch
{
    class TES5ElseSubBranch
    {
        private TES5CodeScope codeScope;
        public TES5ElseSubBranch(TES5CodeScope codeScope = null)
        {
            this.codeScope = codeScope;
        }

        public TES5CodeScope getCodeScope()
        {
            return this.codeScope;
        }
    }
}