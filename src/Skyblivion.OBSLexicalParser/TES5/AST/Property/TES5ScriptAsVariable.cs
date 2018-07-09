using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Property
{
    class TES5ScriptAsVariable : ITES5VariableOrProperty
    {
        private readonly TES5ScriptHeader scriptHeader;

        public TES5ScriptAsVariable(TES5ScriptHeader scriptHeader)
        {
            this.scriptHeader = scriptHeader;
        }

        public string PropertyNameWithSuffix => "self";

        public string Name => "self";

        public IEnumerable<string> Output => new string[] { PropertyNameWithSuffix };

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
            throw new ConversionException("Cannot track TES5ScriptAsVariable as it tracks already.");
        }
    }
}