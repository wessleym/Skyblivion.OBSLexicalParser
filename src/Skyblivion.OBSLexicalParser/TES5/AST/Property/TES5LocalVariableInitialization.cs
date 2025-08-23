using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Property
{
    internal class TES5LocalVariableInitialization : ITES5LocalVariable, ITES5CodeChunk, ITES5OutputtableSingle
    {
        private readonly TES5LocalVariable variable;
        private readonly TES5VariableAssignation assignation;
        public TES5LocalVariableInitialization(TES5LocalVariable variable, TES5VariableAssignation assignation)
        {
            this.variable = variable;
            this.assignation = assignation;
        }

        public string Name => variable.Name;

        public ITES5Type TES5Type { get => variable.TES5Type; set => variable.TES5Type = value; }

        public ITES5Type TES5DeclaredType => variable.TES5DeclaredType;

        public string? ReferenceEDID => variable.ReferenceEDID;

        public void TrackRemoteScript(TES5ScriptHeader scriptHeader)
        {
            variable.TrackRemoteScript(scriptHeader);
        }

        public string OutputSingle => variable.TypeAndName + assignation.EqualsSignValueOutput;

        public IEnumerable<string> Output => [OutputSingle];

        ///This method has only been implemented for <see cref="TES5Float"/>.
        public bool ValueEquals(TES5Float tes5Float)
        {
            TES5Float otherFloat = (TES5Float)assignation.Value;
            return tes5Float.Value == otherFloat.Value;
        }
    }
}
