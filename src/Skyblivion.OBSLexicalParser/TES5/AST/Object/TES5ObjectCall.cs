using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    class TES5ObjectCall : ITES5Referencer, ITES5ObjectAccess, ITES5ValueCodeChunk
    {
        public ITES5Referencer AccessedObject { get; private set; }
        public string FunctionName { get; private set; }
        public TES5ObjectCallArguments Arguments { get; private set; }
        public TES5ObjectCall(ITES5Referencer called, string functionName, TES5ObjectCallArguments arguments)
        {
            this.AccessedObject = called;
            this.FunctionName = functionName;
            this.Arguments = arguments;
        }

        public IEnumerable<string> Output
        {
            get
            {
                string accessedObject = this.AccessedObject.Output.Single();
                string arguments = this.Arguments.Output.Single();
                yield return accessedObject + "." + this.FunctionName + "(" + arguments + ")";
            }
        }

        public ITES5VariableOrProperty? ReferencesTo => null;

        public string Name => "ObjectCall";

        public virtual ITES5Type TES5Type => TES5InheritanceGraphAnalyzer.FindReturnTypeForObjectCall(this.AccessedObject.TES5Type.NativeType, this.FunctionName);
    }
}