using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code
{
    class TES4CodeChunks : ITES4CodeChunk
    {
        public List<ITES4CodeChunk> Chunks { get; }
        public TES4CodeChunks()
        {
            Chunks = new List<ITES4CodeChunk>();
        }

        public void Add(ITES4CodeChunk chunk)
        {
            Chunks.Add(chunk);
        }

        public void AddRange(TES4CodeChunks chunks)
        {
            Chunks.AddRange(chunks.Chunks);
        }

        public bool AreAllComments()
        {
            return Chunks.All(c => c is TES4Comment);
        }
    }
}