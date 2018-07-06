using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Property
{
    class TES5ScriptAsVariable : TES5VariableOrProperty
    {
        private readonly TES5ScriptHeader scriptHeader;
        public TES5ScriptAsVariable(TES5ScriptHeader scriptHeader)
            : base("self")
        {
            this.scriptHeader = scriptHeader;
        }

        public string PropertyNameWithSuffix => "self";

        public override IEnumerable<string> Output => new string[] { PropertyNameWithSuffix };

        public override ITES5Type TES5Type
        {
            get { return this.scriptHeader.ScriptType; }
            set { this.scriptHeader.SetNativeType(value); }
        }

        public override string ReferenceEDID => this.scriptHeader.EDID;

        public override void TrackRemoteScript(TES5ScriptHeader scriptHeader)
        {
            throw new ConversionException("Cannot track TES5ScriptAsVariable as it tracks already.");
        }
    }
}