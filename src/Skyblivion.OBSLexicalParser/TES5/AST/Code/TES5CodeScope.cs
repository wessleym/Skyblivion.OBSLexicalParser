using Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.Context;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Code
{
    /*
     * TES5CodeScope describes scope for given chunks of code. It consists of its variable local scope and chunks that
     * are put inside this code scope.
     * Class TES5CodeScope
     * @package Ormin\OBSLexicalParser\TES5\AST\Code
     */
    class TES5CodeScope : ITES5Outputtable
    {
        private List<ITES5CodeChunk> codeChunks = new List<ITES5CodeChunk>();
        private TES5LocalScope localScope;
        public TES5CodeScope(TES5LocalScope localScope)
        {
            this.localScope = localScope;
        }

        public List<string> output()
        {
            List<string> codeLines = this.localScope.output();
            foreach (var codeChunk in this.codeChunks)
            {
                codeLines.AddRange(codeChunk.output());
            }
            return codeLines;
        }

        public void clear()
        {
            this.codeChunks.Clear();
        }

        public void add(ITES5CodeChunk chunk)
        {
            this.codeChunks.Add(chunk);
        }

        public void addVariable(TES5LocalVariable localVariable)
        {
            this.localScope.addVariable(localVariable);
        }

        public TES5LocalScope getLocalScope()
        {
            return this.localScope;
        }

        public TES5LocalVariable findVariableWithMeaning(TES5LocalVariableParameterMeaning meaning)
        {
            return this.localScope.findVariableWithMeaning(meaning);
        }

        public List<ITES5CodeChunk> getCodeChunks()
        {
            return this.codeChunks;
        }
    }
}