using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Block
{
    class TES5State : TES5CodeBlock
    {
        private readonly string name;
        private readonly bool auto;
        private readonly TES5BlockList codeBlocks;
        public TES5State(string name, bool auto, TES5FunctionScope functionScope, TES5CodeScope codeScope)
        {
            this.name = name;
            this.auto = auto;
            this.FunctionScope = functionScope;
            this.CodeScope = codeScope;
            codeBlocks = new TES5BlockList();
        }

        public override IEnumerable<string> Output =>
            (new string[] { (auto ? "Auto " : "") + "State " + name })
            .Concat(codeBlocks.Output)
            .Concat(new string[] { "EndState" });

        public override TES5CodeScope CodeScope { get; set; }

        public override TES5FunctionScope FunctionScope { get; protected set; }

        public void AddBlock(ITES5CodeBlock block)
        {
            codeBlocks.Add(block);
        }

        public override void AddChunk(ITES5CodeChunk chunk)
        {
            this.CodeScope.AddChunk(chunk);
        }
    }
}
