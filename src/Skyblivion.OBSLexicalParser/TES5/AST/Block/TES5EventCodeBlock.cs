using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Block
{
    class TES5EventCodeBlock : ITES5CodeBlock
    {
        public TES5CodeScope CodeScope { get; set; }
        public TES5FunctionScope FunctionScope { get; private set; }
        public TES5EventCodeBlock(TES5FunctionScope functionScope, TES5CodeScope chunks)
        {
            this.FunctionScope = functionScope;
            this.CodeScope = chunks;
        }

        public IEnumerable<string> Output => (new string[] { "Event " + this.FunctionScope.BlockName+ "(" + string.Join(", ", this.FunctionScope.GetVariablesOutput()) + ")" })
                .Concat(this.CodeScope.Output)
                .Concat(new string[] { "EndEvent" });

        public string BlockType => this.FunctionScope.BlockName;

        public void AddChunk(ITES5CodeChunk chunk)
        {
            this.CodeScope.Add(chunk);
        }
    }
}