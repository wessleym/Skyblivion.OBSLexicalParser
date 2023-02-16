using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Code
{
    class TES5Comment : ITES5CodeChunk
    {
        private readonly string comment;
        public TES5Comment(string comment)
        {
            this.comment = comment;
        }

        //Sometimes comments are just several semicolons: ;;;;;;;;;;
        //Sometimes comments start with two semicolons: ;; some comment
        //If the first character of the comment is semicolon, don't add a space before the comment text.
        public IEnumerable<string> Output => new string[] { ";" + (comment.StartsWith(";") ? "" : " ") + comment };
    }
}
