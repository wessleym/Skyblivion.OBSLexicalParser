using System.Collections;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code
{
    class TES4Comment : ITES4ScriptHeaderVariableDeclarationOrComment, ITES4CodeBlockOrComment
    {
        public string Comment { get; }
        public TES4Comment(string comment)
        {
            Comment = comment;
        }
    }
}
