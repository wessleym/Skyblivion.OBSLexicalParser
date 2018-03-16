using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using System.Collections.Generic;

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

        public List<string> output()
        {
            List<string> codeLines = new List<string>();
            List<string> functionSignatureFlat = new List<string>();
            foreach (var kvp in this.functionScope.getVariables())
            {
                var localVariable = kvp.Value;
                functionSignatureFlat.Add(localVariable.getPropertyType().output() + " " + localVariable.getPropertyName());
            }

            string functionSignature = string.Join(", ", functionSignatureFlat);
            codeLines.Add("Event "+this.functionScope.getBlockName()+"("+functionSignature+")");
            codeLines.AddRange(this.codeScope.output());
            codeLines.Add("EndEvent");
            return codeLines;
        }

        public string getBlockType()
        {
            return this.functionScope.getBlockName();
        }

        public Skyblivion.OBSLexicalParser.TES5.AST.Code.TES5CodeScope getCodeScope()
        {
            return this.codeScope;
        }

        public void setCodeScope(Skyblivion.OBSLexicalParser.TES5.AST.Code.TES5CodeScope codeScope)
        {
            this.codeScope = codeScope;
        }

        public void addChunk(ITES5CodeChunk chunk)
        {
            this.codeScope.add(chunk);
        }

        public Skyblivion.OBSLexicalParser.TES5.AST.Scope.TES5FunctionScope getFunctionScope()
        {
            return this.functionScope;
        }
    }
}