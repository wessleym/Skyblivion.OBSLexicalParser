using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST
{
    class TES4Script : ITES4CodeChunk, IEnumerable<ITES4CodeChunk>
    {
        public TES4ScriptHeader ScriptHeader { get; }
        public List<ITES4ScriptHeaderVariableDeclarationOrComment> VariableDeclarationsAndComments { get; }
        public List<ITES4CodeBlockOrComment> BlockList { get; }
        public TES4Script(TES4ScriptHeader scriptHeader, List<ITES4ScriptHeaderVariableDeclarationOrComment> variableDeclarationsAndComments, List<ITES4CodeBlockOrComment> blockList)
        {
            this.ScriptHeader = scriptHeader;
            this.VariableDeclarationsAndComments = variableDeclarationsAndComments;
            this.BlockList = blockList;
        }

        public IEnumerator<ITES4CodeChunk> GetEnumerator()
        {
            return this.VariableDeclarationsAndComments.Concat<ITES4CodeChunk>(BlockList).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}