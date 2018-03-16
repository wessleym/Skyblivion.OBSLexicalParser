using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    class TES5ObjectCall : ITES5Referencer, ITES5ObjectAccess, ITES5CodeChunk
    {
        private ITES5Referencer called;
        private string functionName;
        private TES5ObjectCallArguments arguments;
        public TES5ObjectCall(ITES5Referencer called, string functionName, TES5ObjectCallArguments arguments = null)
        {
            this.called = called;
            this.functionName = functionName;
            this.arguments = arguments;
        }

        public List<string> output()
        {
            string argumentsCode = "";
            if (this.arguments != null)
            {
                List<string> arguments = this.arguments.output();
                argumentsCode = arguments[0];
            }
            List<string> called = this.called.output();
            string calledFirst = called[0];
            return new List<string>() { called + "." + this.functionName + "(" + argumentsCode + ")" };
        }

        public TES5ObjectCallArguments getArguments()
        {
            return this.arguments;
        }

        public ITES5Referencer getAccessedObject()
        {
            return this.called;
        }

        public string getFunctionName()
        {
            return this.functionName;
        }

        public ITES5Variable getReferencesTo()
        {
            return null;
        }

        public string getName()
        {
            return "ObjectCall";
        }

        public ITES5Type getType()
        {
            return TES5InheritanceGraphAnalyzer.findReturnTypeForObjectCall(this.called.getType(), this.functionName);
        }
    }
}