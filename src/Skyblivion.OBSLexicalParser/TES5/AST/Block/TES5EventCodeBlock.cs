using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Block
{
    class TES5EventCodeBlock : ITES5CodeBlock
    {
        private TES5CodeScope codeScope;
        private TES5FunctionScope functionScope;
        public TES5EventCodeBlock(TES5FunctionScope functionScope, TES5CodeScope chunks)
        {
            this.functionScope = functionScope;
            this.codeScope = chunks;
        }

        public IEnumerable<string> Output => (new string[] { "Event " + this.functionScope.getBlockName() + "(" + string.Join(", ", this.functionScope.getVariablesOutput()) + ")" })
                .Concat(this.codeScope.Output)
                .Concat(new string[] { "EndEvent" });

        public string getBlockType()
        {
            return this.functionScope.getBlockName();
        }

        public TES5CodeScope getCodeScope()
        {
            return this.codeScope;
        }

        public void setCodeScope(TES5CodeScope codeScope)
        {
            this.codeScope = codeScope;
        }

        public void addChunk(ITES5CodeChunk chunk)
        {
            this.codeScope.Add(chunk);
        }

        public TES5FunctionScope getFunctionScope()
        {
            return this.functionScope;
        }
    }
}