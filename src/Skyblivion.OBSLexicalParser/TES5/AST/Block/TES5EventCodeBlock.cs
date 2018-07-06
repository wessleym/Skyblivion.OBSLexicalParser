using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Block
{
    class TES5EventCodeBlock : TES5CodeBlock
    {
        public override TES5CodeScope CodeScope { get; set; }
        public override TES5FunctionScope FunctionScope { get; protected set; }
        public TES5EventCodeBlock(TES5FunctionScope functionScope, TES5CodeScope codeScope)
        {
            this.FunctionScope = functionScope;
            this.CodeScope = codeScope;
        }

        public override IEnumerable<string> Output =>
            (new string[] { "Event " + this.BlockName + "(" + string.Join(", ", this.FunctionScope.GetVariablesOutput()) + ")" })
            .Concat(this.CodeScope.Output)
            .Concat(new string[] { "EndEvent" });

        public override void AddChunk(ITES5CodeChunk chunk)
        {
            this.CodeScope.AddChunk(chunk);
        }
    }
}