using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Context;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Code
{
    /*
     * TES5CodeScope describes scope for given chunks of code. It consists of its variable local scope and chunks that
     * are put inside this code scope.
     */
    class TES5CodeScope : ITES5Outputtable
    {
        public TES5CodeChunkCollection CodeChunks { get; }
        public TES5LocalScope LocalScope { get; }
        public TES5CodeScope(TES5LocalScope localScope)
        {
            this.CodeChunks = new TES5CodeChunkCollection();
            this.LocalScope = localScope;
        }

        public IEnumerable<string> Output => this.LocalScope.Output.Concat(this.CodeChunks.Output);

        public void AddChunk(ITES5CodeChunk chunk)
        {
            this.CodeChunks.Add(chunk);
        }

        public void AddChunks(IEnumerable<ITES5CodeChunk> chunks)
        {
            this.CodeChunks.AddRange(chunks);
        }

        public void AddVariable(TES5LocalVariable localVariable)
        {
            this.LocalScope.AddVariable(localVariable);
        }

        public TES5SignatureParameter? TryGetFunctionParameterByMeaning(TES5LocalVariableParameterMeaning meaning)
        {
            return this.LocalScope.TryGetFunctionParameterByMeaning(meaning);
        }
    }
}