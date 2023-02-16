using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.Types;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;

namespace Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration
{
    class TES4VariableDeclaration : ITES4ScriptHeaderVariableDeclarationOrComment
    {
        public string VariableName { get; }
        public TES4Type VariableType { get; }
        public Nullable<int> FormID { get; }//WTM:  Change:  Added
        public TES5BasicType? TES5Type { get; }//WTM:  Change:  Added
        public TES4Comment? Comment { get; private set; }
        public TES4VariableDeclaration(string variableName, TES4Type variableType, Nullable<int> formID = null, TES5BasicType? tes5Type = null)
        {
            this.VariableName = variableName;
            this.VariableType = variableType;
            FormID = formID;
            TES5Type = tes5Type;
        }

        public void SetComment(TES4Comment comment)
        {
            if (Comment != null) { throw new InvalidOperationException(nameof(Comment) + " was already set."); }
            Comment = comment;
        }
    }
}