using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    class TES5ObjectCall : ITES5Referencer, ITES5ObjectAccess, ITES5ValueCodeChunk
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
                argumentsCode = this.arguments.output().Single();
            }
            string calledFirst = this.called.output().Single();
            return new List<string>() { calledFirst + "." + this.functionName + "(" + argumentsCode + ")" };
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