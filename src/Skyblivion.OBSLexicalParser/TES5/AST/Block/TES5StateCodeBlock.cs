using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Block
{
    class TES5StateCodeBlock : TES5CodeBlock
    {
        public string Name { get; private set; }
        public bool Auto { get; private set; }
        public TES5BlockList CodeBlocks { get; private set; }
        public TES5StateCodeBlock(string name, bool auto, TES5FunctionScope functionScope, TES5CodeScope codeScope)
        {
            this.Name = name;
            this.Auto = auto;
            this.FunctionScope = functionScope;
            this.CodeScope = codeScope;
            CodeBlocks = new TES5BlockList();
        }

        public override IEnumerable<string> Output =>
            (new string[] { (Auto ? "Auto " : "") + "State " + Name })
            .Concat(CodeBlocks.Output)
            .Concat(CodeScope.Output)
            .Concat(new string[] { "EndState" });

        public override TES5CodeScope CodeScope { get; set; }

        public override TES5FunctionScope FunctionScope { get; protected set; }

        public void AddBlock(ITES5CodeBlock block)
        {
            CodeBlocks.Add(block);
        }

        public override void AddChunk(ITES5CodeChunk chunk)
        {
            this.CodeScope.AddChunk(chunk);
        }
    }
}
