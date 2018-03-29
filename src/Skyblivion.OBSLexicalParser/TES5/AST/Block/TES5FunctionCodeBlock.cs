using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Block
{
    class TES5FunctionCodeBlock : ITES5CodeBlock
    {
        private TES5CodeScope codeScope;
        private TES5FunctionScope functionScope;
        private ITES5Type returnType;
        public TES5FunctionCodeBlock(ITES5Type returnType, TES5FunctionScope functionScope, TES5CodeScope chunks)
        {
            this.functionScope = functionScope;
            this.codeScope = chunks;
            this.returnType = returnType;
        }

        public List<string> output()
        {
            List<string> codeLines = new List<string>();
            List<string> functionSignatureFlat = new List<string>();
            foreach (var localVariable in this.functionScope.getVariables().Select(v=>v.Value))
            {
                functionSignatureFlat.Add(localVariable.getPropertyType().output().Single() + " " + localVariable.getPropertyName());
            }

            string functionSignature = string.Join(", ", functionSignatureFlat);
            string functionReturnType = (this.returnType != null) ? this.returnType.value() + " " : "";
            codeLines.Add(functionReturnType + "Function " + this.functionScope.getBlockName() + "(" + functionSignature + ")");
            codeLines.AddRange(this.codeScope.output());
            codeLines.Add("EndFunction");
            return codeLines;
        }

        public string getFunctionName()
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
            this.codeScope.add(chunk);
        }

        public TES5FunctionScope getFunctionScope()
        {
            return this.functionScope;
        }
    }
}