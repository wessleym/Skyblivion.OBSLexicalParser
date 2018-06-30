using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Block
{
    abstract class TES5CodeBlock : ITES5CodeBlock
    {
        public abstract TES5CodeScope CodeScope { get; set; }
        public abstract TES5FunctionScope FunctionScope { get; protected set; }
        public abstract IEnumerable<string> Output { get; }

        public string BlockName => this.FunctionScope.BlockName;

        public abstract void AddChunk(ITES5CodeChunk chunk);
    }
}
