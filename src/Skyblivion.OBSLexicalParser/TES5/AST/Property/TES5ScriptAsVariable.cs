using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Property
{
    class TES5ScriptAsVariable : ITES5VariableOrProperty
    {
        private readonly TES5ScriptHeader scriptHeader;

        public TES5ScriptAsVariable(TES5ScriptHeader scriptHeader)
        {
            this.scriptHeader = scriptHeader;
        }

        public string Name => TES5SelfReference.Self;

        public ITES5Type TES5Type
        {
            get { return this.scriptHeader.ScriptType; }
            set { this.scriptHeader.SetNativeType(value); }
        }

        public ITES5Type TES5DeclaredType
        {
            get
            {
                return TES5Type;
            }
        }

        public string ReferenceEDID => this.scriptHeader.EDID;

        public void TrackRemoteScript(TES5ScriptHeader scriptHeader)
        {
            throw new ConversionException("Cannot track " + nameof(TES5ScriptAsVariable) + " as it tracks already.");
        }
    }
}