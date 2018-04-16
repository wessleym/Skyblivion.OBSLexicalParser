using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Property
{
    class TES5LocalVariable : ITES5Variable
    {
        private string variableName;
        public ITES5Type PropertyType { get; set; }
        public TES5LocalVariableParameterMeaning[] Meanings { get; private set; }
        public TES5LocalVariable(string variableName, TES5BasicType type, TES5LocalVariableParameterMeaning[] meanings = null)
        {
            if (meanings == null) { meanings = new TES5LocalVariableParameterMeaning[] { }; }
            this.variableName = variableName;
            this.PropertyType = type;
            this.Meanings = meanings;
        }

        public IEnumerable<string> Output => new string[] { this.PropertyType.Value+ " " + this.variableName };

        public string PropertyNameWithSuffix => this.variableName;

        /*
        * Todo - following two methods should not be in this interface but TES5Property interface
         * This can be easily done, as the logic is the same as in TES5Property - Set the tracked script,
         * inference information if any available
         * @throws ConversionException
        */
        public string ReferenceEDID => throw new ConversionException("Local variables have no EDID references.");

        public void TrackRemoteScript(TES5ScriptHeader scriptHeader)
        {
            throw new ConversionException("Local variables cannot track remote scripts.");
        }
    }
}