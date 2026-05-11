using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    class TES5ObjectCall : TES5Castable, ITES5Referencer, ITES5ObjectAccess
    {
        public ITES5Referencer AccessedObject { get; }
        public string FunctionName { get; set; }
        public TES5ObjectCallArguments Arguments { get; }
        private readonly TES5Comment? comment;
        public TES5ObjectCall(ITES5Referencer called, string functionName, TES5ObjectCallArguments arguments, TES5Comment? comment)
        {
            this.AccessedObject = called;
            this.FunctionName = functionName;
            this.Arguments = arguments;
            this.comment = comment;
        }

        public string OutputSingle
        {
            get
            {
                string arguments = this.Arguments.OutputSingle;
                string commentOutput = comment != null ? " " + comment.OutputSingle : "";
                return (this.AccessedObject is not TES5SelfReference ? this.AccessedObject.OutputSingle + "." : "") + this.FunctionName + "(" + arguments + ")" + ManualCastToOutput + commentOutput;
            }
        }

        public IEnumerable<string> Output => [OutputSingle];

        public ITES5VariableOrProperty? ReferencesTo => null;

        public string Name => "ObjectCall";

        public virtual ITES5Type TES5Type => TES5InheritanceGraphAnalyzer.FindReturnTypeForObjectCall(this.AccessedObject.TES5Type.NativeType, this.FunctionName);
    }
}