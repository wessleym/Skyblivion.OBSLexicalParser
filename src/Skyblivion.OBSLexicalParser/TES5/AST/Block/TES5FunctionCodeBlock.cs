using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Block
{
    class TES5FunctionCodeBlock : ITES5CodeBlock
    {
        private TES5CodeScope codeScope;
        private TES5FunctionScope functionScope;
        private ITES5Type returnType;
        private bool isStandalone;//Only needed for PHP_COMPAT
        public TES5FunctionCodeBlock(TES5FunctionScope functionScope, TES5CodeScope codeScope, ITES5Type returnType, bool isStandalone = false)
        {
            if (returnType == null) { throw new ArgumentNullException(nameof(returnType)); }
            this.functionScope = functionScope;
            this.codeScope = codeScope;
            this.returnType = returnType;
            this.isStandalone = isStandalone;
        }

        public IEnumerable<string> Output
        {
            get
            {
                string returnTypeValue = this.returnType.Value;
                string functionReturnType = returnTypeValue != "" ? returnTypeValue + " " :
#if PHP_COMPAT
                isStandalone ? "" : " "
#else
                ""
#endif
                ;
                return (new string[] { functionReturnType + "Function " + this.functionScope.getBlockName() + "(" + string.Join(", ", this.functionScope.getVariablesOutput()) + ")" })
                    .Concat(this.codeScope.Output)
                    .Concat(new string[] { "EndFunction" });
            }
        }

        public string getFunctionName()
        {
            return this.functionScope.getBlockName();
        }

        public TES5CodeScope getCodeScope()
        {
            return this.codeScope;
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