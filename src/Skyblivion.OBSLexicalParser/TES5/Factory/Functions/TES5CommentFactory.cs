using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    static class TES5CommentFactory
    {
        public static TES5Comment Construct(TES4Comment comment)
        {
            return new TES5Comment(comment.Comment.Trim());
        }

        public static TES5CodeChunkCollection CreateCodeChunks(IEnumerable<TES4Comment> comments)
        {
            return new TES5CodeChunkCollection(comments.Select(c => Construct(c)));
        }
        public static TES5CodeChunkCollection CreateCodeChunks(TES4Comment comment)
        {
            return CreateCodeChunks(new TES4Comment[] { comment });
        }
    }
}
