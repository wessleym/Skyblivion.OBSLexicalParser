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
        public TES5FunctionScope FunctionScope { get; private set; }
        public TES5CodeScope CodeScope { get; private set; }
        private ITES5Type returnType;
        private bool isStandalone;//Only needed for PHP_COMPAT
        public TES5FunctionCodeBlock(TES5FunctionScope functionScope, TES5CodeScope codeScope, ITES5Type returnType, bool isStandalone = false)
        {
            if (returnType == null) { throw new ArgumentNullException(nameof(returnType)); }
            this.FunctionScope = functionScope;
            this.CodeScope = codeScope;
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
                return (new string[] { functionReturnType + "Function " + this.FunctionScope.BlockName+ "(" + string.Join(", ", this.FunctionScope.GetVariablesOutput()) + ")" })
                    .Concat(this.CodeScope.Output)
                    .Concat(new string[] { "EndFunction" });
            }
        }

        public string getFunctionName()
        {
            return this.FunctionScope.BlockName;
        }

        public void AddChunk(ITES5CodeChunk chunk)
        {
            this.CodeScope.Add(chunk);
        }
    }
}