using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    class TES5SelfReference : TES5Castable, ITES5Referencer
    {
        private readonly TES5ScriptAsVariable scriptAsVariable;
        public TES5SelfReference(TES5ScriptAsVariable scriptAsVariable)
        {
            this.scriptAsVariable = scriptAsVariable;
        }

        public IEnumerable<string> Output => new string[] { "self" + ManualCastToOutput };

        public string Name => "self";

        public ITES5VariableOrProperty ReferencesTo => this.scriptAsVariable;

        public ITES5Type TES5Type => this.scriptAsVariable.TES5Type;
    }
}