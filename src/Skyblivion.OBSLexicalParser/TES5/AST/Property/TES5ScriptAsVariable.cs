using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Property
{
    class TES5ScriptAsVariable : ITES5Variable
    {
        private TES5ScriptHeader scriptHeader;
        public TES5ScriptAsVariable(TES5ScriptHeader scriptHeader)
        {
            this.scriptHeader = scriptHeader;
        }

        public string PropertyNameWithSuffix => "self";

        public IEnumerable<string> Output => new string[] { "self" };

        public ITES5Type PropertyType
        {
            get { return this.scriptHeader.ScriptType; }
            set { this.scriptHeader.setNativeType(value); }
        }

        public string ReferenceEDID => this.scriptHeader.EDID;

        public void TrackRemoteScript(TES5ScriptHeader scriptHeader)
        {
            throw new ConversionException("Cannot track TES5ScriptAsVariable as it tracks already.");
        }
    }
}