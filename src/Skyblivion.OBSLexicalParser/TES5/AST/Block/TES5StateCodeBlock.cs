using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Block
{
    class TES5StateCodeBlock : TES5CodeBlock
    {
        public string Name { get; }
        public bool Auto { get; }
        public List<ITES5CodeBlock> CodeBlocks { get; }
        public TES5StateCodeBlock(string name, bool auto, TES5FunctionScope functionScope, TES5CodeScope codeScope)
        {
            this.Name = name;
            this.Auto = auto;
            this.FunctionScope = functionScope;
            this.CodeScope = codeScope;
            CodeBlocks = new List<ITES5CodeBlock>();
        }

        public override IEnumerable<string> Output
        {
            get
            {
                return new string[] { (Auto ? "Auto " : "") + "State " + Name }
                    .Concat(CodeBlocks.SelectMany((b, i) => (i == 0 ? [] : new string[] { "" }).Concat(b.Output)).Indent())
                    .Concat(CodeScope.Output.Indent())
                    .Concat(new string[] { "EndState" });
            }
        }

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
