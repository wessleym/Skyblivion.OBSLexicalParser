using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Code
{
    class TES5Comment : ITES5CodeChunk, ITES5OutputtableSingle
    {
        private readonly string comment;
        public TES5Comment(string comment)
        {
            this.comment = comment;
        }

        //Sometimes comments are just several semicolons: ;;;;;;;;;;
        //Sometimes comments start with two semicolons: ;; some comment
        //If the first character of the comment is semicolon, don't add a space before the comment text.
        public string OutputSingle => ";" + (comment.StartsWith(";") ? "" : " ") + comment;

        public IEnumerable<string> Output => [OutputSingle];

        public bool ContainsOnlyCommentCharacters => comment.Length > 0 && comment.All(c => c == ';');//If enumerable is empty, All() will return true. So check the length first.
    }
}
